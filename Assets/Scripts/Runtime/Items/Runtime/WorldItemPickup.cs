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
        public enum PickupState
        {
            Available,
            Consumed
        }

        [Header("Item")][SerializeField] ItemDefinition itemDefinition;

        [Min(1)][SerializeField] int quantity = 1;

        [Header("World")][SerializeField] GameObject worldRepresentation;

        [Header("Feedback")]
        [SerializeField] IWorldPickupFeedback feedback;

        PickupState _state = PickupState.Available;

        PickupItemOperation _pickupOperation;

        public Vector3 PickupPosition => transform.position;

        public PickupState State => _state;

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

            feedback?.ResetFeedback();

            if (worldRepresentation != null) worldRepresentation.SetActive(false);
            else gameObject.SetActive(false);
        }

        void OnDisable()
        {
            // If the pickup is disabled for any reason, make sure visual
            // feedback cannot remain applied when it is later re-enabled.
            feedback?.ResetFeedback();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (quantity < 1) quantity = 1;
        }

        public void SetSelected(bool selected)
        {
            feedback.SetHighlighted(selected);
        }
#endif
    }
}