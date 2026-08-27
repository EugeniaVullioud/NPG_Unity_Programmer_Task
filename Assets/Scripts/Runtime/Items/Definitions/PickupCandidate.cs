using UnityEngine;
namespace Game.Items
{
    /// <summary>
    /// Represents a potential pickup target detected by the pickup system.
    /// </summary>
    public readonly struct PickupCandidate
    {
        public readonly IPickupable Pickupable;
        public readonly Collider Collider;
        public readonly Vector3 Position;
        public readonly float Distance;
        public readonly float Angle;

        public PickupCandidate(IPickupable pickupable, Collider collider, Vector3 position, float distance, float angle)
        {
            Pickupable = pickupable;
            Collider = collider;
            Position = position;
            Distance = distance;
            Angle = angle;
        }
    }
}
