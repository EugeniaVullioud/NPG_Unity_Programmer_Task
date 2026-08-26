using Game.Inventory;
using System;
using UnityEngine;

namespace Game.SaveSystem
{
    /// <summary>
    /// Root object representing one complete game save.
    /// Each gameplay subsystem owns a section of this object through its corresponding save participant.
    /// </summary>
    [Serializable]
    public sealed class SaveData
    {
        /// <summary>
        /// Version of the save schema.
        /// Used for future migration support.
        /// </summary>
        public int Version = 1;

        /// <summary>
        /// Persisted player state.
        /// </summary>
        public PlayerSaveData Player = new();

        /// <summary>
        /// Persisted inventory state.
        /// </summary>
        public InventorySaveData Inventory = new();

        /// <summary>
        /// Equipment state.
        /// </summary>
        public EquipmentSaveData Equipment;

        /// <summary>
        /// Creates an empty save.
        /// </summary>
        public static SaveData Create()
        {
            return new SaveData
            {
                Version = 1,
                Inventory = new InventorySaveData(),
                Equipment = new EquipmentSaveData()
            };
        }
    }
}