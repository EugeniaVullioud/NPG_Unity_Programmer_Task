using System;
using System.Collections.Generic;
namespace Game.Inventory
{
    /// <summary>
    /// Persisted equipment-slot assignment.
    /// </summary>
    [Serializable]
    public sealed class EquipmentSlotSaveData
    {
        public int SlotType;
        public string ItemInstanceId;
    }
    /// <summary>
    /// Persisted equipment state.
    /// </summary>
    [Serializable]
    public sealed class EquipmentSaveData
    {
        public List<EquipmentSlotSaveData> slots = new();
    }
}