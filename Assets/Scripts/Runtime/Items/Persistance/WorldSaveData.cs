using System;
using System.Collections.Generic;

namespace Game.Items
{
    /// <summary>
    /// Persisted state for world objects that have changed from their
    /// initial scene state.
    /// </summary>
    [Serializable]
    public sealed class WorldSaveData
    {
        public int Version = 1;
        public List<WorldPickupSaveData> Pickups = new();
    }

    /// <summary>
    /// Persisted representation of one world pickup.
    /// </summary>
    [Serializable]
    public sealed class WorldPickupSaveData
    {
        public string PickupId;
        public bool Consumed;
    }
}