using System;
using System.Collections.Generic;

namespace Game.Inventory
{
    /// <summary>
    /// Root object persisted to disk.
    /// </summary>
    [Serializable]
    public sealed class InventorySaveData
    {
        public int Version = 1;

        /// <summary>
        /// Number of inventory slots.
        /// </summary>
        public int Capacity;

        /// <summary>
        /// Persisted slot data.
        /// </summary>
        public List<InventorySlotSaveData> Slots = new();
    }
}