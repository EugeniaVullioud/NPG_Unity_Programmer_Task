using Game.Items;
using UnityEngine;
namespace Game.Character
{
    /// <summary>
    /// Provides the character ability to detect and pick up nearby objects.
    /// </summary>
    public sealed class PickupAbility : CharacterAbilityBase
    {
        readonly Transform _character;
        readonly Transform _origin;

        float _nextAvailableTime;
        public override AbilityId Id => AbilityId.Pickup;

        public PickupAbility(Transform character, Transform origin, PickupAbilitySettings settings, bool initiallyUnlocked) : base(initiallyUnlocked)
        {
            _character = character;
            _origin = origin;
            _settings = settings;
        }

        public override bool CanExecute()
        {
            if (!IsUnlocked) return false;

            return Time.time >= _nextAvailableTime;
        }

        public override void Execute()
        {
        }
    }
}