using Game.SaveSystem;
using System;

namespace Game.Inventory
{
    /// <summary>
    /// Composes the inventory runtime subsystem from explicitly supplied application dependencies.
    /// This class is Unity integration/composition code. It does not own persistence files or save-slot lifecycle.
    /// </summary>
    public sealed class InventorySystem
    {
        /// <summary>
        /// Gets the authoritative runtime inventory.
        /// </summary>
        public Inventory Inventory { get; private set; }

        /// <summary>
        /// Gets the inventory application facade.
        /// </summary>
        public InventoryService Service { get; private set; }

        /// <summary>
        /// Gets the equipment service.
        /// </summary>
        public EquipmentService Equipment { get; private set; }

        /// <summary>
        /// Gets the item database.
        /// </summary>
        public ItemDatabase ItemDatabase { get; }

        /// <summary>
        /// Gets the modifier database.
        /// </summary>
        public ItemModifierDatabase ModifierDatabase { get; }

        /// <summary>
        /// Gets the inventory save participant.
        /// </summary>
        public InventorySaveParticipant SaveParticipant { get; private set; }

        ConsumableItemUseHandler consumableHandler = new ConsumableItemUseHandler();

        private readonly SaveManager saveManager;
        private readonly int inventoryCapacity;

        /// <summary>
        /// Creates the inventory subsystem.
        /// </summary>
        public InventorySystem(ItemDatabase itemDatabase, ItemModifierDatabase modifierDatabase, SaveManager saveManager, int inventoryCapacity)
        {
            ItemDatabase = itemDatabase ?? throw new ArgumentNullException(nameof(itemDatabase));
            ModifierDatabase = modifierDatabase ?? throw new ArgumentNullException(nameof(modifierDatabase));

            this.saveManager = saveManager ?? throw new ArgumentNullException(nameof(saveManager));
            this.inventoryCapacity = inventoryCapacity;
        }
        /// <summary>
        /// Creates and connects the runtime inventory subsystem.
        /// </summary>
        public void Initialize()
        {
            Inventory = new Inventory(inventoryCapacity);

            Equipment = new EquipmentService(Inventory, ItemDatabase);

            InventoryMutationService mutations = new InventoryMutationService(Inventory, ItemDatabase);

            ItemActionService actions = new ItemActionService(Inventory, mutations, ItemDatabase);
            actions.RegisterHandler(consumableHandler);

            Service = new InventoryService(Inventory, mutations, actions, Equipment);

            SaveParticipant = new InventorySaveParticipant(Inventory, ItemDatabase);

            saveManager.Register(SaveParticipant);
        }
    }
}
