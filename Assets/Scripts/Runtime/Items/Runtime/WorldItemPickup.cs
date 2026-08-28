using Game.Inventory;
using System;
using UnityEngine;
namespace Game.Items
{
    /// <summary>
    /// World representation of an inventory item that can be picked up.
    /// This component owns world-pickup lifecycle and delegates inventory mutation.
    /// </summary>
    public sealed class WorldItemPickup : MonoBehaviour, IPickupable, IPickupSelectionFeedback
    {
        [Header("Persistence")]
        [SerializeField] string _pickupId;

        public string PickupId => _pickupId;
        public enum PickupState
        {
            Available,
            Consumed
        }
        [Header("Detection")]
        [SerializeField] Collider[] _colliders;

        [Header("Item")]
        [SerializeField] ItemDefinition itemDefinition;

        [Min(1)][SerializeField] int quantity = 1;

        [Header("World")][SerializeField] GameObject worldRepresentation;

        [Header("Feedback")]
        [SerializeField] Transform _feedbackProvider;

        IWorldPickupFeedback feedback;

        PickupState _state = PickupState.Available;

        PickupItemOperation _pickupOperation;

        public Vector3 PickupPosition => transform.position;

        public PickupState State => _state;

        void Awake()
        {
            feedback = _feedbackProvider.GetComponentInChildren<IWorldPickupFeedback>();
            if (feedback == null) throw new ArgumentNullException(nameof(feedback));
        }
        /// <summary>
        /// Installs application dependencies. This is intentionally explicit rather than looking up a global InventorySystem from the pickup.
        /// </summary>
        public void Initialize(PickupItemOperation pickupOperation)
        {
            _pickupOperation = pickupOperation ?? throw new ArgumentNullException(nameof(pickupOperation));
        }

        public bool CanBePickedUp(in PickupContext context)
        {
            return _state == PickupState.Available && _pickupOperation != null && itemDefinition != null && quantity > 0;
        }

        public bool TryPickUp(in PickupContext context)
        {
            if (!CanBePickedUp(context)) return false;

            bool success = _pickupOperation.TryPickup(itemDefinition, quantity);

            // Inventory full, invalid item, etc.
            // Crucially, the world pickup remains available.
            if (!success) return false;

            Consume();

            return true;
        }

        void Consume()
        {
            // Set state before disabling anything. This makes repeated interaction attempts harmless even if disabling causes other callbacks to run.
            _state = PickupState.Consumed;
            EnableCollision(false);
            feedback?.ResetFeedback();

            if (worldRepresentation != null) worldRepresentation.SetActive(false);
            else gameObject.SetActive(false);
        }
        void EnableCollision(bool state)
        {
            foreach (var collider in _colliders)
            {
                collider.enabled = state;
            }
        }
        void OnDisable()
        {
            // If the pickup is disabled for any reason, make sure visual
            // feedback cannot remain applied when it is later re-enabled.
            feedback?.ResetFeedback();
            EnableCollision(true);
        }
        public void SetSelected(bool selected)
        {
            feedback.SetHighlighted(selected);
        }
        internal void RestoreConsumed()
        {
            if (_state == PickupState.Consumed) return;

            _state = PickupState.Consumed;

            EnableCollision(false);

            feedback?.ResetFeedback();

            if (worldRepresentation != null) worldRepresentation.SetActive(false);
            else gameObject.SetActive(false);
        }
        internal void RestoreAvailable()
        {
            _state = PickupState.Available;

            EnableCollision(true);

            if (worldRepresentation != null) worldRepresentation.SetActive(true);
        }
#if UNITY_EDITOR
        void OnValidate()
        {
            if (quantity < 1) quantity = 1;

            if (string.IsNullOrWhiteSpace(_pickupId))
            {
                Debug.LogWarning($"WorldItemPickup '{name}' requires a unique Pickup ID.", this);
            }
        }
#endif
    }
}