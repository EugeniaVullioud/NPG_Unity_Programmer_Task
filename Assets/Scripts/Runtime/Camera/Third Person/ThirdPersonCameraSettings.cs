using UnityEngine;
namespace Game.Camera
{
    /// <summary>
    /// Configures distance, rotation, and collision settings for a third-person camera.
    /// </summary>
    [CreateAssetMenu(fileName = "ThirdPersonCameraSettings", menuName = "Character/Camera/Third Person Camera Settings")]
    public sealed class ThirdPersonCameraSettings : ScriptableObject
    {
        [Header("Distance")]
        [SerializeField, Min(0.1f)] float _minimumDistance = 0.5f;

        [SerializeField, Min(0.1f)] float _maximumDistance = 4f;

        [Header("Rotation")][SerializeField] float _minimumPitch = -70f;

        [SerializeField] float _maximumPitch = 75f;

        [Header("Collision")]
        [SerializeField, Min(0.01f)] float _collisionRadius = 0.2f;
        [SerializeField, Min(0f)] float _collisionOffset = 0.05f;
        [SerializeField] LayerMask _collisionMask = ~0;

        public float MinimumDistance => _minimumDistance;

        public float MaximumDistance => _maximumDistance;

        public float MinimumPitch => _minimumPitch;

        public float MaximumPitch => _maximumPitch;

        public float CollisionRadius => _collisionRadius;

        public float CollisionOffset => _collisionOffset;

        public LayerMask CollisionMask => _collisionMask;

        void OnValidate()
        {
            _minimumDistance = Mathf.Max(0.01f, _minimumDistance);

            _maximumDistance = Mathf.Max(_minimumDistance, _maximumDistance);

            _maximumPitch = Mathf.Max(_minimumPitch, _maximumPitch);
        }
    }
}