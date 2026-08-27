using System;
namespace Game.Inventory
{
    /// <summary>
    /// Creates runtime inventory items from static item definitions.
    /// </summary>
    public sealed class ItemInstanceFactory
    {
        public ItemInstance Create(ItemDefinition definition, int quantity)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));

            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            return new ItemInstance(definition.Id, quantity);
        }
    }
}