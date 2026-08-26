using System;

namespace Game.Inventory
{
    /// <summary>
    /// Owns equipment rules and coordinates equipment with inventory state. 
    /// Can be extended in the future by implementing rules.
    /// </summary>
    public sealed class EquipmentService
    {
        readonly Inventory inventory;
        readonly ItemDatabase itemDatabase;

        /// <summary>
        /// Gets the authoritative equipment state.
        /// </summary>
        public EquipmentState State { get; }

        /// <summary>
        /// Creates an equipment service.
        /// </summary>
        public EquipmentService(Inventory inventory, ItemDatabase itemDatabase)
        {
            this.inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            this.itemDatabase = itemDatabase ?? throw new ArgumentNullException(nameof(itemDatabase));

            State = new EquipmentState();
        }

        /// <summary>
        /// Attempts to equip an item from an inventory slot.
        /// </summary>
        public bool Equip(int inventorySlot, EquipmentSlotType equipmentSlot)
        {
            InventorySlot slot = inventory.TryGetSlot(inventorySlot);

            if (slot == null || slot.IsEmpty) return false;

            ItemInstance item = slot.Item;

            if (!itemDatabase.TryGet(item.DefinitionId, out ItemDefinition definition)) return false;

            if (!definition.IsEquippable ||
                State.TryGetEquipped(equipmentSlot, out string existing)) return false;

            State.Equip(equipmentSlot, item.InstanceId);

            return true;
        }

        /// <summary>
        /// Unequips an equipment slot.
        /// </summary>
        public bool Unequip(EquipmentSlotType equipmentSlot)
        {
            return State.Unequip(equipmentSlot, out _);
        }

        /// <summary>
        /// Attempts to retrieve the runtime item currently equipped.
        /// </summary>
        public bool TryGetEquippedItem(EquipmentSlotType slot, out ItemInstance item)
        {
            item = null;

            if (!State.TryGetEquipped(slot, out string instanceId)) return false;
            return inventory.TryGetItem(instanceId, out item);
        }
    }
}