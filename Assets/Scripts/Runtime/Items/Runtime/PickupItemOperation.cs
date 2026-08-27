using System;

namespace Game.Inventory
{
    public sealed class PickupItemOperation
    {
        readonly IPickupInventoryReceiver _inventory;
        readonly ItemInstanceFactory _factory;

        public PickupItemOperation(IPickupInventoryReceiver inventory, ItemInstanceFactory factory)
        {
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));

            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public bool TryPickup(ItemDefinition definition, int quantity)
        {
            if (definition == null || quantity <= 0) return false;

            ItemInstance item = _factory.Create(definition, quantity);

            return _inventory.TryAdd(item);
        }
    }
}