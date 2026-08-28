using Game.SaveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Inventory
{
    /// <summary>
    /// Serializes and restores inventory runtime state as part of the save system.
    /// Unity assets are represented by persistent IDs rather than being duplicated into save data.
    /// </summary>
    public sealed class InventorySaveParticipant : ISaveParticipant
    {
        const int CurrentVersion = 1;

        readonly Inventory inventory;
        readonly ItemDatabase itemDatabase;
        readonly ItemModifierDatabase modifierDatabase;

        /// <inheritdoc />
        public string Id => "inventory";

        public InventorySaveParticipant(Inventory inventory, ItemDatabase itemDatabase)
        {
            this.inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            this.itemDatabase = itemDatabase ?? throw new ArgumentNullException(nameof(itemDatabase));
        }

        /// <inheritdoc />
        public void Capture(SaveData saveData)
        {
            if (saveData == null)
            {
                throw new ArgumentNullException(nameof(saveData));
            }
            InventorySaveData data = new InventorySaveData
            {
                Version = CurrentVersion,
                Capacity = inventory.Capacity
            };

            for (int i = 0; i < inventory.Capacity; i++)
            {
                InventorySlot slot = inventory.TryGetSlot(i);

                InventorySlotSaveData slotData = new InventorySlotSaveData { SlotIndex = i };

                if (!slot.IsEmpty)
                {
                    ItemInstance item = slot.Item;

                    slotData.InstanceId = item.InstanceId;

                    slotData.DefinitionId = item.DefinitionId;

                    slotData.Quantity = item.Quantity;
                }

                data.Slots.Add(slotData);
            }

            saveData.Inventory = data;
        }

        /// <inheritdoc />
        public bool Restore(SaveData saveData)
        {
            if (saveData == null || saveData.Inventory == null)
            {
                throw new ArgumentNullException(nameof(saveData));
            }

            InventorySaveData data = saveData.Inventory;

            if (data.Version > CurrentVersion) return false;

            if (data.Capacity != inventory.Capacity) return false;


            HashSet<string> itemIds = new(StringComparer.Ordinal);

            inventory.ClearWithoutEvents();

            for (int i = 0; i < data.Slots.Count; i++)
            {
                InventorySlotSaveData slotData = data.Slots[i];

                if (slotData == null) continue;
                if (slotData.SlotIndex < 0 || slotData.SlotIndex >= inventory.Capacity) continue;


                if (string.IsNullOrWhiteSpace(slotData.InstanceId) ||
                    string.IsNullOrWhiteSpace(slotData.DefinitionId)) continue;

                if (!itemIds.Add(slotData.InstanceId)) continue;
                if (!itemDatabase.TryGet(slotData.DefinitionId, out ItemDefinition definition)) continue;
                if (slotData.Quantity <= 0) continue;


                int quantity = Math.Min(slotData.Quantity, definition.MaxStackSize);


                ItemInstance item = new ItemInstance(slotData.InstanceId, slotData.DefinitionId, quantity);

                inventory.RestoreSlot(slotData.SlotIndex, item);
            }

            inventory.RaiseLoaded();
            return true;
        }

    }
}
