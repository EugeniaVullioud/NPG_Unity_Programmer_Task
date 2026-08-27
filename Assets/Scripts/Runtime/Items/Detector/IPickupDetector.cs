using System.Collections.Generic;
using UnityEngine;
namespace Game.Items
{
    /// <summary>
    /// Detects pickupable items within a specified area or direction.
    /// </summary>
    public interface IPickupDetector
    {
        /// <summary>
        /// Detects pickup candidates from the specified origin and forward direction.
        /// </summary>
        /// <returns>
        /// The number of pickup candidates detected.
        /// </returns>
        int Detect(Vector3 origin, Vector3 forward, List<PickupCandidate> results);
    }
}