using UnityEngine;

namespace Game.Character
{
    [CreateAssetMenu(fileName = "CharacterMovementSettings", menuName = "Character/Movement Settings")]
    public sealed class CharacterMovementSettings : ScriptableObject
    {
        [Header("Speed")]
        [SerializeField, Min(0f)] float _maximumSpeed = 6f;

        [Header("Acceleration")]
        [SerializeField, Min(0f)] float _acceleration = 30f;
        [SerializeField, Min(0f)] float _deceleration = 40f;

        [Header("Rotation")]
        [SerializeField, Min(0f)] float _rotationSpeed = 720f;

        public float MaximumSpeed => _maximumSpeed;
        public float Acceleration => _acceleration;
        public float Deceleration => _deceleration;
        public float RotationSpeed => _rotationSpeed;
    }
}