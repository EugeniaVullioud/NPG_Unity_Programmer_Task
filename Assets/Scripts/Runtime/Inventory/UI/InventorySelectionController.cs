using System;
using UnityEngine;

namespace Game.Inventory
{
    /// <summary>
    /// Owns UI selection state for inventory items.
    /// Selection is based on ItemInstance identity rather than slot position.
    /// </summary>
    public class InventorySelectionController
    {
        readonly Inventory inventory;

        string selectedInstanceId;

        /// <summary>
        /// Gets the instance ID of the currently selected item,
        /// or null when nothing is selected.
        /// </summary>
        public string SelectedInstanceId => selectedInstanceId;

        /// <summary>
        /// Gets whether an item is currently selected.
        /// </summary>
        public bool HasSelection => !string.IsNullOrEmpty(selectedInstanceId);

        /// <summary>
        /// Creates a selection controller for an inventory.
        /// </summary>
        public InventorySelectionController(Inventory inventory)
        {
            this.inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));

            inventory.Changed += OnInventoryChanged;
        }

        /// <summary>
        /// Selects the item occupying the specified slot.
        /// Clicking the selected slot again clears selection.
        /// Empty slots cannot be selected.
        /// </summary>
        public bool Toggle(int slotIndex)
        {
            InventorySlot slot = inventory.TryGetSlot(slotIndex);

            if (slot == null || slot.IsEmpty)
            {
                return false;
            }

            string instanceId = slot.Item.InstanceId;

            if (selectedInstanceId == instanceId)
            {
                Clear();
                return false;
            }

            selectedInstanceId = instanceId;
            SelectionChanged?.Invoke();

            return true;
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        public void Clear()
        {
            if (selectedInstanceId == null)
            {
                return;
            }

            selectedInstanceId = null;
            SelectionChanged?.Invoke();
        }

        /// <summary>
        /// Attempts to resolve the currently selected item.
        /// </summary>
        public bool TryGetSelectedItem(out ItemInstance item)
        {
            if (string.IsNullOrEmpty(selectedInstanceId))
            {
                item = null;
                return false;
            }

            return inventory.TryGetItem(selectedInstanceId, out item);
        }

        /// <summary>
        /// Attempts to resolve the current slot of the selected item.
        /// </summary>
        public bool TryGetSelectedSlot(out int slotIndex)
        {
            if (string.IsNullOrEmpty(selectedInstanceId))
            {
                slotIndex = -1;
                return false;
            }

            return inventory.TryGetItemSlot(selectedInstanceId, out slotIndex);
        }

        /// <summary>
        /// Raised whenever selection changes.
        /// </summary>
        public event Action SelectionChanged;

        void OnInventoryChanged(InventoryChangedEventArgs change)
        {
            if (string.IsNullOrEmpty(selectedInstanceId))
            {
                return;
            }

            if (!inventory.TryGetItem(selectedInstanceId, out _))
            {
                Clear();
            }
            else
            {
                // The item may have moved to another slot.
                // Its identity remains the same, so selection itself does not change.
                SelectionChanged?.Invoke();
            }
        }

        /// <summary>
        /// Releases the inventory event subscription.
        /// </summary>
        public void Dispose()
        {
            inventory.Changed -= OnInventoryChanged;
        }
    }
}