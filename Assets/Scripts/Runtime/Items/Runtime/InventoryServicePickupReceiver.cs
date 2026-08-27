using System;
namespace Game.Inventory
{
    /// <summary>
    /// Adapts <see cref="InventoryService"/> to the
    /// <see cref="IPickupInventoryReceiver"/> interface.
    /// </summary>
    /// <remarks>
    /// This adapter allows callers that only know about <see cref="IPickupInventoryReceiver"/> to add items to an inventory
    /// without depending directly on <see cref="InventoryService"/>.
    /// </remarks>
    public sealed class InventoryServicePickupReceiver : IPickupInventoryReceiver
    {
        readonly InventoryService _service;

        public InventoryServicePickupReceiver(InventoryService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }
        /// <summary>
        /// Attempts to add an item to the inventory.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the item was successfully added; otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryAdd(ItemInstance item)
        {
            InventoryMutationResult result = _service.Add(item, out _);

            return result.Success;
        }
    }
}