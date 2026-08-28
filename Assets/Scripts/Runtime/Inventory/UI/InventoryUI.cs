using Game.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    /// <summary>
    /// Coordinates the visual inventory representation. The UI listens to inventory events and updates only the affected slots rather than rebuilding the entire UI every frame.
    /// </summary>
    public sealed class InventoryUI : BaseUI
    {
        /// <summary>
        /// Gets the slot currently being dragged.
        /// </summary>
        public static int DraggedSlot { get; set; } = -1;

        [SerializeField] InventoryDragController dragController;
        [SerializeField] InventorySlotUI slotPrefab;
        [SerializeField] Transform slotContainer;

        InventorySystem inventorySystem;

        InventorySlotUI[] slots;

        void OnDisable()
        {
            if (inventorySystem == null || inventorySystem.Inventory == null) return;

            inventorySystem.Inventory.Changed -= OnInventoryChanged;
        }

        void CreateSlots()
        {
            int slotCount = inventorySystem.Inventory.Capacity;

            slots = new InventorySlotUI[slotCount];

            for (int i = 0; i < slotCount; i++)
            {
                InventorySlotUI slot = Instantiate(slotPrefab, slotContainer);

                slot.Bind(i, inventorySystem.Service, inventorySystem.ItemDatabase);
                slot.Bind(dragController);
                slots[i] = slot;
            }
        }
        public void Initialize(InventorySystem system)
        {
            inventorySystem = system;

            CreateSlots();

            inventorySystem.Inventory.Changed += OnInventoryChanged;

            RefreshAll();
        }
        void OnInventoryChanged(InventoryChangedEventArgs change)
        {
            switch (change.ChangeType)
            {
                case InventoryChangeType.Moved:

                    RefreshSlot(change.SlotIndex);

                    RefreshSlot(change.SecondarySlotIndex);

                    break;

                case InventoryChangeType.Swapped:

                    RefreshSlot(change.SlotIndex);

                    RefreshSlot(change.SecondarySlotIndex);

                    break;

                case InventoryChangeType.ItemChanged:

                    RefreshSlot(change.SlotIndex);

                    break;

                case InventoryChangeType.Split:

                    RefreshSlot(change.SlotIndex);

                    RefreshSlot(change.SecondarySlotIndex);

                    break;

                case InventoryChangeType.Cleared:

                    RefreshAll();

                    break;
            }
        }

        void RefreshSlot(int index)
        {
            if (index < 0 || index >= slots.Length)
            {
                return;
            }

            slots[index].Refresh();
        }

        void RefreshAll()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Refresh();
            }
        }
    }
}