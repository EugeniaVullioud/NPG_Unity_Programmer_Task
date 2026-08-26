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

        //public InventorySaveParticipant(Inventory inventory, ItemDatabase itemDatabase, ItemModifierDatabase modifierDatabase)
        public InventorySaveParticipant(Inventory inventory, ItemDatabase itemDatabase)
        {
            this.inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            //this.modifierDatabase = modifierDatabase ?? throw new ArgumentNullException(nameof(inventory));
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

                    slotData.Durability = item.Durability;

                    for (int m = 0; m < item.Modifiers.Count; m++)
                    {
                        ItemModifierInstance modifier = item.Modifiers[m];

                        slotData.Modifiers.Add(new ItemModifierSaveData
                        {
                            InstanceId = modifier.InstanceId,
                            DefinitionId = modifier.DefinitionId,
                            Value = modifier.Value
                        });
                    }
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

                float durability = definition.HasDurability ? Math.Clamp(slotData.Durability, 0f, definition.MaximumDurability) : 0f;

                ItemInstance item = new ItemInstance(
                        slotData.InstanceId,
                        slotData.DefinitionId,
                        quantity
                        // durability
                        );

                List<ItemModifierInstance> modifiers = new();

                HashSet<string> modifierIds = new(StringComparer.Ordinal);

                if (slotData.Modifiers != null)
                {
                    for (int m = 0; m < slotData.Modifiers.Count; m++)
                    {
                        ItemModifierSaveData modifierData = slotData.Modifiers[m];

                        if (modifierData == null) continue;

                        if (string.IsNullOrWhiteSpace(modifierData.InstanceId) ||
                            string.IsNullOrWhiteSpace(modifierData.DefinitionId)) continue;


                        if (!modifierIds.Add(modifierData.InstanceId)) continue;
                        if (!modifierDatabase.TryGet(modifierData.DefinitionId, out _)) continue;

                        modifiers.Add(new ItemModifierInstance
                            (
                                modifierData.InstanceId,
                                modifierData.DefinitionId,
                                modifierData.Value)
                            );
                    }
                }

                item.RestoreModifiers(modifiers);
                inventory.RestoreSlot(slotData.SlotIndex, item);
            }

            inventory.RaiseLoaded();
            return true;
        }

    }
}
