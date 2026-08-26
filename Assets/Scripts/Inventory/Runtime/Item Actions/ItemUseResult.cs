namespace Game.Inventory
{
    /// <summary>
    /// Identifies why an item could not be used.
    /// </summary>
    public enum ItemUseFailure
    {
        None,
        ItemNotFound,
        NoHandler,
        CannotUse
    }
    /// <summary>
    /// Represents the result of attempting to use an item.
    /// </summary>
    public readonly struct ItemUseResult
    {
        /// <summary>
        /// Gets whether the item was successfully used.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets whether the item should be consumed.
        /// </summary>
        public bool ConsumeItem { get; }

        /// <summary>
        /// Gets the failure reason.
        /// </summary>
        public ItemUseFailure Failure { get; }

        private ItemUseResult(bool success, bool consumeItem, ItemUseFailure failure)
        {
            Success = success;
            ConsumeItem = consumeItem;
            Failure = failure;
        }

        /// <summary>
        /// Creates a successful use result.
        /// </summary>
        public static ItemUseResult Used(bool consumeItem = true)
        {
            return new ItemUseResult(true, consumeItem, ItemUseFailure.None);
        }

        /// <summary>
        /// Creates a failed use result.
        /// </summary>
        public static ItemUseResult Failed(ItemUseFailure failure)
        {
            return new ItemUseResult(false, false, failure);
        }
    }
}