using System;
using System.Collections.Generic;

namespace Game.Inventory
{
    /// <summary>
    /// Identifies an equipment position.
    /// </summary>
    public enum EquipmentSlotType
    {
        LeftHand,
        RightHand,
        Head,
        Chest,
        Legs,
        Feet,
        Accessory
    }

    /// <summary>
    /// Authoritative runtime equipment state.Equipment stores item instance identifiers rather than duplicating items.
    /// </summary>
    public sealed class EquipmentState
    {
        readonly Dictionary<EquipmentSlotType, string> equipped = new();

        /// <summary>
        /// Raised when equipment changes.
        /// </summary>
        public event Action<EquipmentSlotType, string> Changed;

        /// <summary>
        /// Attempts to retrieve the item instance ID equipped in a slot.
        /// </summary>
        public bool TryGetEquipped(EquipmentSlotType slot, out string instanceId)
        {
            return equipped.TryGetValue(slot, out instanceId);
        }

        internal void Equip(EquipmentSlotType slot, string instanceId)
        {
            equipped[slot] = instanceId;

            Changed?.Invoke(slot, instanceId);
        }

        internal bool Unequip(EquipmentSlotType slot, out string instanceId)
        {
            if (!equipped.TryGetValue(slot, out instanceId)) return false;

            equipped.Remove(slot);
            Changed?.Invoke(slot, null);

            return true;
        }

        internal IReadOnlyDictionary<EquipmentSlotType, string> GetState()
        {
            return equipped;
        }

        internal void Clear()
        {
            equipped.Clear();
        }
    }
}
