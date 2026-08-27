using UnityEngine;
namespace Game.Character
{
    public readonly struct GroundState
    {
        /// <summary>
        /// Gets whether the character is currently supported by walkable ground.
        /// </summary>
        public bool IsGrounded { get; }

        /// <summary>
        /// Gets whether walkable ground was detected within the nearby ground range.
        /// </summary>
        public bool HasNearbyGround { get; }

        /// <summary>
        /// Gets the raycast hit produced by the ground probe.
        /// </summary>
        public RaycastHit Hit { get; }


        public Vector3 Normal { get; }

        public float Distance { get; }

        public Collider Collider => Hit.collider;

        public Rigidbody Rigidbody => Hit.rigidbody;

        public GroundState(bool isGrounded, bool hasNearbyGround, RaycastHit hit, Vector3 normal, float distance)
        {
            IsGrounded = isGrounded;
            HasNearbyGround = hasNearbyGround;
            Hit = hit;
            Normal = normal;
            Distance = distance;
        }
        /// <summary>
        /// Gets a ground state representing the absence of detected ground.
        /// </summary>
        public static GroundState NotGrounded => new GroundState(false, false, default, Vector3.up, float.PositiveInfinity);
    }
}