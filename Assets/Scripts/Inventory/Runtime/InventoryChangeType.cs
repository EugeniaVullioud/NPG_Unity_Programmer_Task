namespace Game.Inventory
{
    /// <summary>
    /// Identifies the kind of inventory change.
    /// </summary>
    public enum InventoryChangeType
    {
        Added,
        Removed,
        Moved,
        Swapped,
        Split,
        ItemChanged,
        Cleared,
    }

    /// <summary>
    /// Describes a single inventory mutation or item state change.
    /// </summary>
    public readonly struct InventoryChangedEventArgs
    {
        /// <summary>
        /// Gets the type of change.
        /// </summary>
        public InventoryChangeType ChangeType { get; }

        /// <summary>
        /// Gets the primary affected slot.
        /// </summary>
        public int SlotIndex { get; }

        /// <summary>
        /// Gets the secondary affected slot when applicable.
        /// </summary>
        public int SecondarySlotIndex { get; }

        /// <summary>
        /// Gets the affected item.
        /// </summary>
        public ItemInstance Item { get; }

        /// <summary>
        /// Gets the previous item when applicable.
        /// </summary>
        public ItemInstance PreviousItem { get; }

        /// <summary>
        /// Creates inventory change information.
        /// </summary>
        public InventoryChangedEventArgs(InventoryChangeType changeType, int slotIndex, int secondarySlotIndex, ItemInstance item, ItemInstance previousItem)
        {
            ChangeType = changeType;
            SlotIndex = slotIndex;
            SecondarySlotIndex = secondarySlotIndex;
            Item = item;
            PreviousItem = previousItem;
        }
    }
}
