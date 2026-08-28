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
        [SerializeField] ItemDetailsPanel itemDetailsPanel;

        InventorySystem inventorySystem;
        InventorySelectionController selectionController;
        InventoryDetailsController detailsController;

        InventorySlotUI[] slots;

        void OnDisable()
        {
            if (inventorySystem == null || inventorySystem.Inventory == null) return;

            inventorySystem.Inventory.Changed -= OnInventoryChanged;

            detailsController?.Dispose();
            selectionController?.Dispose();

            detailsController = null;
            selectionController = null;

            DraggedSlot = -1;
            dragController?.End();
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
                slot.Bind(selectionController, detailsController);
                slots[i] = slot;
            }
        }
        public void Initialize(InventorySystem system, InventorySelectionController selectionController)
        {
            inventorySystem = system;
            this.selectionController = selectionController;

            detailsController = new InventoryDetailsController(inventorySystem.Inventory, inventorySystem.ItemDatabase, itemDetailsPanel, selectionController);
            CreateSlots();

            inventorySystem.Inventory.Changed += OnInventoryChanged;

            RefreshAll();
            RefreshSelectionVisuals();
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

                case InventoryChangeType.Added:
                case InventoryChangeType.Removed:
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
            if (index < 0 || index >= slots.Length) return;

            slots[index].Refresh();
        }

        void RefreshAll()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Refresh();
            }
        }
        void RefreshSelectionVisuals()
        {
            if (selectionController == null) return;

            selectionController.TryGetSelectedSlot(out int selectedSlot);

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].SetSelected(i == selectedSlot);
            }
        }
        /// <summary>
        /// Attempts to use the currently selected inventory item.
        /// The item-use system determines whether the item can be used and whether using it consumes inventory quantity.
        /// </summary>
        public ItemUseResult UseSelected()
        {
            if (selectionController == null)
            {
                return ItemUseResult.Failed(ItemUseFailure.ItemNotFound);
            }

            if (!selectionController.TryGetSelectedSlot(out int slotIndex))
            {
                return ItemUseResult.Failed(ItemUseFailure.ItemNotFound);
            }

            return inventorySystem.Service.Actions.Use(slotIndex);
        }
    }
}