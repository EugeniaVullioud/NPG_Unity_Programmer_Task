using Game.Items;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Character
{
    /// <summary>
    /// Provides the character ability to detect and pick up nearby objects.
    /// </summary>
    public sealed class PickupAbility : CharacterAbilityBase, IFixedUpdateAbility
    {
        readonly Transform _character;
        readonly Transform _origin;
        readonly PickupAbilitySettings _settings;

        readonly IPickupDetector _detector;
        readonly IPickupSelector _selector;
        public override AbilityId Id => AbilityId.Pickup;

        // Runtime
        readonly List<PickupCandidate> _candidates = new();
        IPickupSelectionFeedback _selectedFeedback;

        float _nextAvailableTime;

        public PickupAbility(Transform character, IPickupDetector detector, IPickupSelector selector, Transform origin, PickupAbilitySettings settings, bool initiallyUnlocked) : base(initiallyUnlocked)
        {
            _character = character;
            _origin = origin;
            _settings = settings;

            _detector = detector;
            _selector = selector;
        }
        public override bool CanExecute()
        {
            if (!IsUnlocked) return false;

            return Time.time >= _nextAvailableTime;
        }

        public override void Execute()
        {
            if (!CanExecute()) return;

            if (!_selector.TrySelect(_candidates, out PickupCandidate candidate)) return;

            PickupContext context = new PickupContext(_character);

            if (!candidate.Pickupable.TryPickUp(in context)) return;

            _nextAvailableTime = Time.time + _settings.Cooldown;
        }

        public void FixedUpdateAbility(float deltaTime)
        {
            // Could be improved by only firing when player has significantly moved, or it could also have a DetectionInterval = 0.05f;
            // which means doing detection at 20 Hz. But it's overcomplicating a solution. 
            if (!IsUnlocked)
            {
                ClearSelection();
                return;
            }

            Vector3 origin = _origin.position;
            Vector3 forward = _character.forward;

            _detector.Detect(origin, forward, _candidates);

            UpdateSelection();
        }
        void UpdateSelection()
        {
            if (!_selector.TrySelect(_candidates, out PickupCandidate candidate))
            {
                ClearSelection();
                return;
            }

            IPickupSelectionFeedback feedback = candidate.Pickupable as IPickupSelectionFeedback;

            if (ReferenceEquals(feedback, _selectedFeedback)) return;

            ClearSelection();

            _selectedFeedback = feedback;
            _selectedFeedback?.SetSelected(true);
        }

        void ClearSelection()
        {
            if (_selectedFeedback == null) return;

            _selectedFeedback.SetSelected(false);
            _selectedFeedback = null;
        }
    }
}