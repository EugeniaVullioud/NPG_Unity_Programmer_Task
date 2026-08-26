namespace Game.Inventory
{
    /// <summary>
    /// Handles gameplay behavior associated with using an item.
    /// </summary>
    public interface IItemUseHandler
    {
        /// <summary>
        /// Determines whether this handler can use the item.
        /// </summary>
        bool CanHandle(ItemInstance item, ItemDefinition definition);

        /// <summary>
        /// Executes the item's use behavior.
        /// </summary>
        ItemUseResult Use(ItemInstance item, ItemDefinition definition);
    }
}