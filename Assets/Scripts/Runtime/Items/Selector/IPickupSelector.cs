using System.Collections.Generic;
namespace Game.Items
{
    /// <summary>
    /// Selects a single pickup candidate from a collection of detected candidates.
    /// </summary>
    public interface IPickupSelector
    {
        /// <summary>
        /// Attempts to select a pickup candidate from the provided candidates.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if a candidate was successfully selected; otherwise, <see langword="false"/>.
        /// </returns>
        bool TrySelect(IReadOnlyList<PickupCandidate> candidates, out PickupCandidate selected);
    }
}
