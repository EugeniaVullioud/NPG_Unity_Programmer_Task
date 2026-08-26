using System;
namespace Game.Inventory
{
    /// <summary>
    /// Application-facing facade for the inventory subsystem.
    /// This class does not own inventory state. It coordinates the specialized services that operate on that state.
    /// </summary>
    public sealed class InventoryService
    {
        /// <summary>
        /// Gets the authoritative runtime inventory.
        /// </summary>
        public Inventory Inventory { get; }

        /// <summary>
        /// Gets inventory mutation operations.
        /// </summary>
        public InventoryMutationService Mutation { get; }

        /// <summary>
        /// Gets item-use operations.
        /// </summary>
        public ItemActionService Actions { get; }

        /// <summary>
        /// Gets equipment operations.
        /// </summary>
        public EquipmentService Equipment { get; }

        /// <summary>
        /// Creates the inventory application service.
        /// </summary>
        public InventoryService(Inventory inventory, InventoryMutationService mutation, ItemActionService actions, EquipmentService equipment)
        {
            Inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));

            Mutation = mutation ?? throw new ArgumentNullException(nameof(mutation));

            Actions = actions ?? throw new ArgumentNullException(nameof(actions));

            Equipment = equipment ?? throw new ArgumentNullException(nameof(equipment));
        }
    }
}