using UnityEngine;
namespace Game.Character
{
    [CreateAssetMenu(fileName = "PickupAbilitySettings", menuName = "Character/Abilities/Pickup Settings")]
    public sealed class PickupAbilitySettings : ScriptableObject
    {
        [SerializeField, Min(0f)] float _range = 2f;

        [SerializeField, Range(0f, 180f)] float _maximumAngle = 60f;

        [SerializeField] LayerMask _pickupMask;

        [SerializeField, Min(0f)] float _cooldown = 0.15f;
        [SerializeField, Min(1)] int _maxDetectionColliders = 15;

        public float Range => _range;

        public float MaximumAngle => _maximumAngle;

        public LayerMask PickupMask => _pickupMask;

        public float Cooldown => _cooldown;
        public int MaxDetectionColliders => _maxDetectionColliders;
    }
}
