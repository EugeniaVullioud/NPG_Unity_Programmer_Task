namespace Game.Inventory
{
    /// <summary>
    /// Handles the use of consumable items.
    /// </summary>
    public sealed class ConsumableItemUseHandler : IItemUseHandler
    {
        /// <summary>
        /// Determines whether this handler can process the specified item.
        /// </summary>
        public bool CanHandle(ItemInstance item, ItemDefinition definition)
        {
            return item != null && definition != null && definition.Category == ItemCategory.Consumable;
        }

        public ItemUseResult Use(ItemInstance item, ItemDefinition definition)
        {
            return ItemUseResult.Used(consumeItem: true);
        }
    }
}