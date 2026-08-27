using UnityEngine;
namespace Game.Items
{
    /// <summary>
    /// Provides contextual information for a pickup operation.
    /// </summary>
    public readonly struct PickupContext
    {
        /// <summary>
        /// Gets the character performing the pickup.
        /// </summary>
        public Transform Character { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PickupContext"/> struct.
        /// </summary>
        /// <param name="character">The character performing the pickup.</param>
        public PickupContext(Transform character)
        {
            Character = character;
        }
    }
}