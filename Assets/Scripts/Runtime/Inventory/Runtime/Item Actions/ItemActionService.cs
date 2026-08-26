using System;
using System.Collections.Generic;

namespace Game.Inventory
{
    /// <summary>
    /// Coordinates item use requests without involving gameplay-specific behavior into inventory domain objects.
    /// </summary>
    public sealed class ItemActionService
    {
        readonly Inventory inventory;
        readonly ItemDatabase itemDatabase;

        readonly List<IItemUseHandler> handlers;

        {
            this.inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            this.mutationService = mutationService ?? throw new ArgumentNullException(nameof(mutationService));
            this.itemDatabase = itemDatabase ?? throw new ArgumentNullException(nameof(itemDatabase));

            this.handlers = handlers != null
                    ? new List<IItemUseHandler>(handlers)
                    : new List<IItemUseHandler>();
        }

        /// <summary>
        /// Registers an item-use handler.
        /// </summary>
        public void RegisterHandler(IItemUseHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        /// <summary>
        /// Attempts to use the item occupying a slot.
        /// </summary>
        public ItemUseResult Use(int slotIndex)
        {
            InventorySlot slot = inventory.TryGetSlot(slotIndex);

            if (slot == null || slot.IsEmpty)
            {
                return ItemUseResult.Failed(ItemUseFailure.ItemNotFound);
            }

            ItemInstance item = slot.Item;

            if (!itemDatabase.TryGet(item.DefinitionId, out ItemDefinition definition))
            {
                return ItemUseResult.Failed(ItemUseFailure.ItemNotFound);
            }

            for (int i = 0; i < handlers.Count; i++)
            {
                IItemUseHandler handler = handlers[i];

                if (!handler.CanHandle(item, definition)) continue;

                ItemUseResult result = handler.Use(item, definition);

                if (result.Success && result.ConsumeItem)
                {
                    ConsumeOne(slotIndex, item);
                }
                return result;
            }
            return ItemUseResult.Failed(ItemUseFailure.NoHandler);
        }

        void ConsumeOne(int slotIndex, ItemInstance item)
        {
            if (item.Quantity > 1)
            {
                item.SetQuantity(item.Quantity - 1);
                return;
            }

            mutationService.Remove(slotIndex, out _);
        }
    }
}