using System;

namespace Game.Inventory
{
    /// <summary>
    /// Persisted representation of one inventory slot.
    /// </summary>
    [Serializable]
    public sealed class InventorySlotSaveData
    {
        /// <summary>
        /// Persistent slot index.
        /// </summary>
        public int SlotIndex;

        /// <summary>
        /// Unique item instance identifier.
        /// </summary>
        public string InstanceId;

        /// <summary>
        /// Item definition identifier.
        /// </summary>
        public string DefinitionId;

        /// <summary>
        /// Current quantity.
        /// </summary>
        public int Quantity;

    }
}