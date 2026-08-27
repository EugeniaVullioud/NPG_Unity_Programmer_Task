using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// Defines an object that can be picked up.
    /// </summary>
    public interface IPickupable
    {
        Vector3 PickupPosition {get;} 

        /// <summary>
        /// Determines whether this object can be picked up in the specified context.
        /// </summary>
        /// <param name="context">The context in which the pickup is attempted.</param>
        /// <returns><see langword="true"/> if the object can be picked up; otherwise, <see langword="false"/>.</returns>
        bool CanBePickedUp(in PickupContext context);

        /// <summary>
        /// Picks up this object using the specified context.
        /// </summary>
        /// <param name="context">The context in which the pickup is performed.</param>
        bool TryPickUp(in PickupContext context);
    }
}