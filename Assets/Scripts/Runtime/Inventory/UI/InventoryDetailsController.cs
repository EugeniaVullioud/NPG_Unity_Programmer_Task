using System;
namespace Game.Inventory
{
    /// <summary>
    /// Coordinates inventory hover/selection state with the item details presentation.
    /// </summary>
    public sealed class InventoryDetailsController
    {
        readonly Inventory inventory;
        readonly ItemDatabase database;
        readonly ItemDetailsPanel panel;

        int hoveredSlotIndex = -1;

        InventorySelectionController selection;

        public InventoryDetailsController(Inventory inventory, ItemDatabase database, ItemDetailsPanel panel, InventorySelectionController selection)
        {
            this.inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            this.database = database ?? throw new ArgumentNullException(nameof(database));
            this.panel = panel ?? throw new ArgumentNullException(nameof(panel));
            this.selection = selection ?? throw new ArgumentNullException(nameof(selection));

            selection.SelectionChanged += Refresh;

            inventory.Changed += OnInventoryChanged;
        }

        /// <summary>
        /// Sets the slot currently under the pointer.
        /// </summary>
        public void SetHoveredSlot(int slotIndex)
        {
            hoveredSlotIndex = slotIndex;
            Refresh();
        }

        /// <summary>
        /// Clears the current hover state.
        /// </summary>
        public void ClearHoveredSlot(int slotIndex)
        {
            if (hoveredSlotIndex != slotIndex) return;
            hoveredSlotIndex = -1;
            Refresh();
        }

        void OnInventoryChanged(InventoryChangedEventArgs change)
        {
            Refresh();
        }

        void Refresh()
        {
            if (TryGetHoveredItem(out ItemInstance hoveredItem))
            {
                Show(hoveredItem);
                return;
            }

            if (selection.TryGetSelectedItem(out ItemInstance selectedItem))
            {
                Show(selectedItem);
                return;
            }

            panel.Hide();
        }

        bool TryGetHoveredItem(out ItemInstance item)
        {
            item = null;

            if (hoveredSlotIndex < 0) return false;

            InventorySlot slot = inventory.TryGetSlot(hoveredSlotIndex);

            if (slot == null || slot.IsEmpty) return false;

            item = slot.Item;

            return database.TryGet(item.DefinitionId, out _);
        }

        void Show(ItemInstance item)
        {
            if (!database.TryGet(item.DefinitionId, out ItemDefinition definition))
            {
                panel.Hide();
                return;
            }

            panel.Show(item, definition);
        }

        /// <summary>
        /// Releases event subscriptions.
        /// </summary>
        public void Dispose()
        {
            selection.SelectionChanged -= Refresh;
            inventory.Changed -= OnInventoryChanged;
        }
    }
}