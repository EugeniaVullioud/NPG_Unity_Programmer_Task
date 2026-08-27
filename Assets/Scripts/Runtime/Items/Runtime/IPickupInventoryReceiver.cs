namespace Game.Inventory
{
    /// <summary>
    /// Application boundary used by world pickups to transfer an item into the player's inventory.
    /// </summary>
    public interface IPickupInventoryReceiver
    {
        bool TryAdd(ItemInstance item);
    }
}