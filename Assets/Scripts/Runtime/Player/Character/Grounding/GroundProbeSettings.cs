using UnityEngine;
namespace Game.Character
{
    [CreateAssetMenu(fileName = "GroundProbeSettings", menuName = "Character/Ground Probe Settings")]
    public sealed class GroundProbeSettings : ScriptableObject
    {
        [SerializeField] LayerMask _groundMask;

        [SerializeField, Min(0f)] float _probeRadius = 0.25f;

        [SerializeField, Min(0f)] float _supportDistance = 0.1f;

        [SerializeField, Min(0f)] float _nearGroundDistance = 0.5f;

        [SerializeField, Range(0f, 89f)] float _maximumSlopeAngle = 50f;

        public LayerMask GroundMask => _groundMask;

        public float ProbeRadius => _probeRadius;

        /// <summary>
        /// Gets the maximum distance at which a surface provides ground support.
        /// </summary>
        public float SupportDistance => _supportDistance;

        /// <summary>
        /// Gets the maximum distance at which nearby ground is detected.
        /// </summary>
        public float NearGroundDistance => _nearGroundDistance;

        public float MaximumSlopeAngle => _maximumSlopeAngle;

        /// <summary>
        /// Gets the minimum dot product required for a surface to be considered walkable.
        /// </summary>
        public float MinimumGroundDot => Mathf.Cos(_maximumSlopeAngle * Mathf.Deg2Rad);

        /// <summary>
        /// Validates the probe distances and ensures nearby ground detection reaches at least as far as the support distance.
        /// </summary>
        void OnValidate()
        {
            _nearGroundDistance = Mathf.Max(_nearGroundDistance, _supportDistance);
        }
    }
}