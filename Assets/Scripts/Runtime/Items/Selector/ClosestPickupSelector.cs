using System.Collections.Generic;
namespace Game.Items
{
    /// <summary>
    /// Selects the pickup candidate closest to the selection origin.
    /// </summary>
    public sealed class ClosestPickupSelector : IPickupSelector
    {
        /// <summary>
        /// Attempts to select the closest pickup candidate from the provided candidates.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if a candidate was selected; otherwise, <see langword="false"/> when no candidates are available.
        /// </returns>
        public bool TrySelect(IReadOnlyList<PickupCandidate> candidates, out PickupCandidate selected)
        {
            selected = default;

            if (candidates.Count == 0) return false;

            float closestDistance = float.MaxValue;

            foreach (PickupCandidate candidate in candidates)
            {
                if (candidate.Distance >= closestDistance) continue;

                closestDistance = candidate.Distance;
                selected = candidate;
            }

            return true;
        }
    }
}