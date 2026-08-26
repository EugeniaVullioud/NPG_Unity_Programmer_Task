namespace Game.Inventory
{
    /// <summary>
    /// Represents a fixed inventory position. The slot index is persistent and is intentionally independent of the item instance.
    /// </summary>
    public sealed class InventorySlot
    {
        ItemInstance item;

        /// <summary>
        /// Gets the persistent slot index.
        /// </summary>
        public int Index { get; }

        public bool IsEmpty => item == null;

        /// <summary>
        /// Gets the item currently occupying this slot.
        /// </summary>
        public ItemInstance Item =>  item;

        internal InventorySlot(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Assigns an item to the slot.
        /// </summary>
        internal void SetItem(ItemInstance item)
        {
            this.item = item;
        }

        /// <summary>
        /// Removes and returns the item from this slot.
        /// </summary>
        internal ItemInstance Clear()
        {
            ItemInstance previous = item;
            item = null;
            return previous;
        }
    }
}

