#if UNITY_EDITOR

using Game.Inventory;
using NUnit.Framework;
using UnityEngine;

public class Test_InventoryMutation
{
    /// <summary>
    /// Verifies that adding a valid ItemInstance to an empty inventory
    /// places the item into an available slot and reports a successful mutation.
    /// The ItemDatabase is populated with the item's ItemDefinition so that
    /// the mutation service can successfully resolve the item's DefinitionId.
    /// </summary>
    [Test]
    public void Add_PlacesItemInEmptySlot()
    {
        // Arrange
        ItemDefinition definition = CreateDefinition("health_potion", 5);
        ItemDatabase database = CreateDatabase(definition);

        ItemInstance item =
            new ItemInstance("health_potion", 1);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        // Act
        InventoryMutationResult result =
            mutation.Add(item, out int slot);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.GreaterOrEqual(slot, 0);
        Assert.Less(slot, inventory.Capacity);

        Assert.AreSame(
            item,
            inventory.TryGetSlot(slot).Item);
    }

    /// <summary>
    /// Verifies that moving an item updates the inventory's instance-to-slot
    /// lookup so that the item's new slot can be resolved by its InstanceId.
    /// </summary>
    [Test]
    public void Move_UpdatesItemSlotLookup()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        ItemInstance item =
            new ItemInstance("health_potion", 1);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        InventoryMutationResult addResult =
            mutation.Add(item, out int originalSlot);

        Assert.IsTrue(addResult.Success);

        // Act
        InventoryMutationResult moveResult =
            mutation.Move(originalSlot, 3);

        // Assert
        Assert.IsTrue(moveResult.Success);

        Assert.IsTrue(
            inventory.TryGetItemSlot(
                item.InstanceId,
                out int movedSlot));

        Assert.AreEqual(3, movedSlot);
    }

    /// <summary>
    /// Verifies that moving an entire stack transfers the complete ItemInstance
    /// to the destination slot without changing its quantity or InstanceId.
    /// </summary>
    [Test]
    public void Move_MovesEntireStack()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        ItemInstance item =
            new ItemInstance("health_potion", 3);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        InventoryMutationResult addResult =
            mutation.Add(item, out int originalSlot);

        Assert.IsTrue(addResult.Success);

        // Act
        InventoryMutationResult moveResult =
            mutation.Move(originalSlot, 2);

        // Assert
        Assert.IsTrue(moveResult.Success);

        Assert.IsNull(
            inventory.TryGetSlot(originalSlot).Item);

        ItemInstance movedItem =
            inventory.TryGetSlot(2).Item;

        Assert.IsNotNull(movedItem);

        Assert.AreEqual(3, movedItem.Quantity);
        Assert.AreEqual(item.InstanceId, movedItem.InstanceId);
        Assert.AreEqual(item.DefinitionId, movedItem.DefinitionId);
    }

    /// <summary>
    /// Verifies that splitting one item from a stack leaves the remaining
    /// quantity in the original stack and creates a separate ItemInstance
    /// containing the requested quantity.
    /// </summary>
    [Test]
    public void Split_CanSplitStack()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        ItemInstance item =
            new ItemInstance("health_potion", 4);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        InventoryMutationResult addResult =
            mutation.Add(item, out int originalSlot);

        Assert.IsTrue(addResult.Success);

        // Act
        InventoryMutationResult splitResult =
            mutation.Split(originalSlot, 2, 1);

        // Assert
        Assert.IsTrue(splitResult.Success);

        ItemInstance originalItem =
            inventory.TryGetSlot(originalSlot).Item;

        ItemInstance splitItem =
            inventory.TryGetSlot(2).Item;

        Assert.IsNotNull(originalItem);
        Assert.IsNotNull(splitItem);

        Assert.AreEqual(3, originalItem.Quantity);
        Assert.AreEqual(1, splitItem.Quantity);

        Assert.AreNotEqual(
            item.InstanceId,
            splitItem.InstanceId);

        Assert.AreEqual(
            item.DefinitionId,
            splitItem.DefinitionId);

        Assert.AreEqual(
            4,
            originalItem.Quantity + splitItem.Quantity);
    }

    /// <summary>
    /// Verifies that splitting multiple items from a stack correctly
    /// subtracts the requested quantity and places that quantity into
    /// a new ItemInstance.
    /// </summary>
    [Test]
    public void Split_CanSplitMultipleItems()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 10);

        ItemDatabase database =
            CreateDatabase(definition);

        ItemInstance item =
            new ItemInstance("health_potion", 8);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        Assert.IsTrue(
            mutation.Add(item, out int originalSlot).Success);

        // Act
        InventoryMutationResult result =
            mutation.Split(originalSlot, 2, 3);

        // Assert
        Assert.IsTrue(result.Success);

        Assert.AreEqual(
            5,
            inventory.TryGetSlot(originalSlot).Item.Quantity);

        Assert.AreEqual(
            3,
            inventory.TryGetSlot(2).Item.Quantity);
    }

    /// <summary>
    /// Verifies that adding an item whose definition does not exist in the
    /// ItemDatabase fails without modifying the inventory.
    /// </summary>
    [Test]
    public void Add_RejectsUnknownDefinition()
    {
        // Arrange
        ItemDefinition knownDefinition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(knownDefinition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance item =
            new ItemInstance("does_not_exist", 1);

        // Act
        InventoryMutationResult result =
            mutation.Add(item, out int slot);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(-1, slot);

        Assert.IsFalse(
            inventory.TryGetItem(
                item.InstanceId,
                out _));
    }

    /// <summary>
    /// Verifies that adding the same ItemInstance twice is rejected and
    /// does not create a second copy of the item in the inventory.
    /// </summary>
    [Test]
    public void Add_RejectsItemAlreadyInInventory()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        ItemInstance item =
            new ItemInstance("health_potion", 1);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        Assert.IsTrue(
            mutation.Add(item, out int originalSlot).Success);

        // Act
        InventoryMutationResult result =
            mutation.Add(item, out int secondSlot);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(-1, secondSlot);

        Assert.AreEqual(
            item.InstanceId,
            inventory.TryGetSlot(originalSlot).Item.InstanceId);
    }

    /// <summary>
    /// Verifies that moving an item to the same slot is rejected and
    /// leaves the inventory unchanged.
    /// </summary>
    [Test]
    public void Move_RejectsSameSlot()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance item =
            new ItemInstance("health_potion", 1);

        Assert.IsTrue(
            mutation.Add(item, out int slot).Success);

        // Act
        InventoryMutationResult result =
            mutation.Move(slot, slot);

        // Assert
        Assert.IsFalse(result.Success);

        Assert.AreSame(
            item,
            inventory.TryGetSlot(slot).Item);
    }

    /// <summary>
    /// Verifies that moving from an invalid negative slot is rejected
    /// without modifying the inventory.
    /// </summary>
    [Test]
    public void Move_RejectsNegativeSourceSlot()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        // Act
        InventoryMutationResult result =
            mutation.Move(-1, 2);

        // Assert
        Assert.IsFalse(result.Success);
    }

    /// <summary>
    /// Verifies that moving to an index outside the inventory capacity
    /// is rejected without modifying the inventory.
    /// </summary>
    [Test]
    public void Move_RejectsDestinationOutsideCapacity()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance item =
            new ItemInstance("health_potion", 1);

        Assert.IsTrue(
            mutation.Add(item, out int slot).Success);

        // Act
        InventoryMutationResult result =
            mutation.Move(slot, inventory.Capacity);

        // Assert
        Assert.IsFalse(result.Success);

        Assert.AreSame(
            item,
            inventory.TryGetSlot(slot).Item);
    }

    /// <summary>
    /// Verifies that moving one compatible stack onto another merges the two
    /// stacks correctly. The destination stack receives the source quantity,
    /// the source slot becomes empty, and the destination ItemInstance remains
    /// registered in the inventory.
    /// </summary>
    [Test]
    public void Move_MergesCompatibleStacks()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance item =
            new ItemInstance("health_potion", 2);

        Assert.IsTrue(
            mutation.Add(item, out int originalSlot).Success);

        Assert.IsTrue(mutation.Split(originalSlot, 1, 1).Success);

        InventorySlot sourceSlot = inventory.TryGetSlot(originalSlot);

        InventorySlot destinationSlot = inventory.TryGetSlot(1);

        Assert.IsNotNull(sourceSlot);
        Assert.IsNotNull(destinationSlot);

        Assert.IsNotNull(sourceSlot.Item);
        Assert.IsNotNull(destinationSlot.Item);

        Assert.AreEqual(1, sourceSlot.Item.Quantity);
        Assert.AreEqual(1, destinationSlot.Item.Quantity);

        // Remember the destination instance because it should survive the merge.
        ItemInstance destinationItem =
            destinationSlot.Item;

        // Act
        InventoryMutationResult result =
            mutation.Move(originalSlot, 1);

        // Assert
        Assert.IsTrue(result.Success);

        // The source stack was completely consumed.
        Assert.IsNull(sourceSlot.Item);

        // The destination stack contains both quantities.
        Assert.IsNotNull(destinationSlot.Item);
        Assert.AreEqual(2, destinationSlot.Item.Quantity);

        // The destination ItemInstance survives.
        Assert.AreSame(
            destinationItem,
            destinationSlot.Item);

        // The source item was removed from the inventory.
        Assert.IsFalse(
            inventory.TryGetItem(item.InstanceId, out _));

        Assert.IsFalse(
            inventory.TryGetItemSlot(item.InstanceId, out _));

        // The destination item is still correctly registered.
        Assert.IsTrue(
            inventory.TryGetItem(
                destinationItem.InstanceId,
                out ItemInstance registered));

        Assert.AreSame(
            destinationItem,
            registered);

        Assert.IsTrue(
            inventory.TryGetItemSlot(
                destinationItem.InstanceId,
                out int destinationSlotIndex));

        Assert.AreEqual(
            1,
            destinationSlotIndex);
    }
    /// <summary>
    /// Verifies that moving from an empty slot fails and does not modify
    /// any inventory slot.
    /// </summary>
    [Test]
    public void Move_RejectsEmptySource()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        // Act
        InventoryMutationResult result =
            mutation.Move(0, 2);

        // Assert
        Assert.IsFalse(result.Success);

        Assert.IsTrue(
            inventory.TryGetSlot(0).IsEmpty);

        Assert.IsTrue(
            inventory.TryGetSlot(2).IsEmpty);
    }

    /// <summary>
    /// Verifies that splitting zero items is rejected without modifying
    /// the source stack or destination slot.
    /// </summary>
    [Test]
    public void Split_RejectsZeroAmount()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance item =
            new ItemInstance("health_potion", 4);

        Assert.IsTrue(
            mutation.Add(item, out int sourceSlot).Success);

        // Act
        InventoryMutationResult result =
            mutation.Split(sourceSlot, 2, 0);

        // Assert
        Assert.IsFalse(result.Success);

        Assert.AreEqual(
            4,
            inventory.TryGetSlot(sourceSlot).Item.Quantity);

        Assert.IsTrue(
            inventory.TryGetSlot(2).IsEmpty);
    }

    /// <summary>
    /// Verifies that splitting a negative quantity is rejected without
    /// modifying the source stack or destination slot.
    /// </summary>
    [Test]
    public void Split_RejectsNegativeAmount()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance item =
            new ItemInstance("health_potion", 4);

        Assert.IsTrue(
            mutation.Add(item, out int sourceSlot).Success);

        // Act
        InventoryMutationResult result =
            mutation.Split(sourceSlot, 2, -1);

        // Assert
        Assert.IsFalse(result.Success);

        Assert.AreEqual(
            4,
            inventory.TryGetSlot(sourceSlot).Item.Quantity);

        Assert.IsTrue(
            inventory.TryGetSlot(2).IsEmpty);
    }

    /// <summary>
    /// Verifies that attempting to split an entire stack is rejected,
    /// because Split is intended to leave at least one item in the
    /// original stack.
    /// </summary>
    [Test]
    public void Split_RejectsEntireStack()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance item =
            new ItemInstance("health_potion", 4);

        Assert.IsTrue(
            mutation.Add(item, out int sourceSlot).Success);

        // Act
        InventoryMutationResult result =
            mutation.Split(sourceSlot, 2, 4);

        // Assert
        Assert.IsFalse(result.Success);

        Assert.AreEqual(
            4,
            inventory.TryGetSlot(sourceSlot).Item.Quantity);

        Assert.IsTrue(
            inventory.TryGetSlot(2).IsEmpty);
    }

    /// <summary>
    /// Verifies that splitting into an occupied destination fails and
    /// leaves the original stack unchanged.
    /// </summary>
    [Test]
    public void Split_RejectsOccupiedDestination()
    {
        // Arrange
        ItemDefinition definition = CreateDefinition("health_potion", 5);

        ItemDatabase database = CreateDatabase(definition);

        Inventory inventory = new Inventory(5);

        InventoryMutationService mutation = new InventoryMutationService(inventory, database);

        ItemInstance first = new ItemInstance("health_potion", 4);

        ItemInstance second = new ItemInstance("health_potion", 1);

        Assert.IsTrue(mutation.Add(first, out int sourceSlot).Success);
        Assert.IsTrue(mutation.Add(second, out int destinationSlot).Success);

        // Slip into a an empty slot
        Assert.IsTrue(mutation.Split(sourceSlot, 2, 2).Success);

        InventorySlot source = inventory.TryGetSlot(sourceSlot);
        InventorySlot destination = inventory.TryGetSlot(2);

        // Act
        InventoryMutationResult result = mutation.Split(sourceSlot, 2, 1);

        // Assert
        Assert.IsFalse(result.Success);

        Assert.AreEqual(3, inventory.TryGetSlot(sourceSlot).Item.Quantity);
        Assert.AreEqual(2, inventory.TryGetSlot(2).Item.Quantity);

        Assert.AreSame(destination.Item, inventory.TryGetSlot(2).Item);
    }

    /// <summary>
    /// Verifies that splitting a non-stackable item is rejected and
    /// leaves the original item unchanged.
    /// </summary>
    [Test]
    public void Split_RejectsNonStackableItem()
    {
        // Arrange
        ItemDefinition definition = CreateDefinition("sword", 1);

        ItemDatabase database = CreateDatabase(definition);

        Inventory inventory = new Inventory(5);

        InventoryMutationService mutation = new InventoryMutationService(inventory, database);

        ItemInstance item = new ItemInstance("sword", 1);

        Assert.IsTrue(
            mutation.Add(item, out int sourceSlot).Success);

        // Act
        InventoryMutationResult result = mutation.Split(sourceSlot, 2, 1);

        // Assert
        Assert.IsFalse(result.Success);

        Assert.AreSame(item, inventory.TryGetSlot(sourceSlot).Item);

        Assert.IsTrue(inventory.TryGetSlot(2).IsEmpty);
    }

    /// <summary>
    /// Verifies that adding compatible items combines their quantities
    /// into an existing stack rather than creating an unnecessary new slot.
    /// </summary>
    [Test]
    public void Add_StacksCompatibleItems()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance first =
            new ItemInstance("health_potion", 2);

        ItemInstance second =
            new ItemInstance("health_potion", 2);

        Assert.IsTrue(
            mutation.Add(first, out int firstSlot).Success);

        // Act
        InventoryMutationResult result =
            mutation.Add(second, out int resultingSlot);

        // Assert
        Assert.IsTrue(result.Success);

        Assert.AreEqual(
            4,
            inventory.TryGetSlot(firstSlot).Item.Quantity);

        Assert.IsTrue(
            inventory.TryGetItemSlot(
                first.InstanceId,
                out int resolvedSlot));

        Assert.AreEqual(
            firstSlot,
            resolvedSlot);
    }

    /// <summary>
    /// Verifies that adding an item to a full stack creates a new stack
    /// rather than exceeding the configured maximum stack size.
    /// </summary>
    [Test]
    public void Add_FullStack_CreatesNewStack()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance first =
            new ItemInstance("health_potion", 5);

        ItemInstance second =
            new ItemInstance("health_potion", 1);

        Assert.IsTrue(
            mutation.Add(first, out int firstSlot).Success);

        // Act
        InventoryMutationResult result =
            mutation.Add(second, out int secondSlot);

        // Assert
        Assert.IsTrue(result.Success);

        Assert.AreEqual(
            5,
            inventory.TryGetSlot(firstSlot).Item.Quantity);

        Assert.AreEqual(
            1,
            inventory.TryGetSlot(secondSlot).Item.Quantity);

        Assert.AreNotEqual(
            firstSlot,
            secondSlot);
    }

    /// <summary>
    /// Verifies that adding more items than one stack can contain creates
    /// multiple stacks while preserving the total quantity of the added item.
    /// </summary>
    [Test]
    public void Add_SplitsLargeQuantityAcrossStacks()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance item =
            new ItemInstance("health_potion", 12);

        // Act
        InventoryMutationResult result =
            mutation.Add(item, out int firstSlot);

        // Assert
        Assert.IsTrue(result.Success);

        int totalQuantity = 0;

        for (int i = 0; i < inventory.Capacity; i++)
        {
            InventorySlot slot =
                inventory.TryGetSlot(i);

            if (!slot.IsEmpty &&
                slot.Item.DefinitionId == "health_potion")
            {
                totalQuantity += slot.Item.Quantity;
            }
        }

        Assert.AreEqual(12, totalQuantity);
    }

    /// <summary>
    /// Verifies that an item with a quantity larger than the maximum stack
    /// size is divided into valid stacks without exceeding the configured
    /// maximum quantity of any individual stack.
    /// </summary>
    [Test]
    public void Add_NeverExceedsMaximumStackSize()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance item =
            new ItemInstance("health_potion", 11);

        // Act
        InventoryMutationResult result =
            mutation.Add(item, out _);

        // Assert
        Assert.IsTrue(result.Success);

        for (int i = 0; i < inventory.Capacity; i++)
        {
            InventorySlot slot =
                inventory.TryGetSlot(i);

            if (slot.IsEmpty) continue;

            Assert.LessOrEqual(
                slot.Item.Quantity,
                definition.MaxStackSize);
        }
    }

    /// <summary>
    /// Verifies that splitting a stack preserves the total quantity of items,
    /// ensuring that the operation neither creates nor destroys items.
    /// </summary>
    [Test]
    public void Split_PreservesTotalQuantity()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 10);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance item =
            new ItemInstance("health_potion", 7);

        Assert.IsTrue(
            mutation.Add(item, out int sourceSlot).Success);

        // Act
        InventoryMutationResult result =
            mutation.Split(sourceSlot, 3, 2);

        // Assert
        Assert.IsTrue(result.Success);

        int originalQuantity =
            inventory.TryGetSlot(sourceSlot).Item.Quantity;

        int splitQuantity =
            inventory.TryGetSlot(3).Item.Quantity;

        Assert.AreEqual(
            7,
            originalQuantity + splitQuantity);
    }

    /// <summary>
    /// Verifies that removing an item returns the same ItemInstance that
    /// was stored in the inventory and leaves its slot empty.
    /// </summary>
    [Test]
    public void Remove_ReturnsOriginalItemAndClearsSlot()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        ItemInstance item =
            new ItemInstance("health_potion", 2);

        Assert.IsTrue(
            mutation.Add(item, out int slot).Success);

        // Act
        InventoryMutationResult result =
            mutation.Remove(
                slot,
                out ItemInstance removedItem);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreSame(item, removedItem);

        Assert.IsTrue(
            inventory.TryGetSlot(slot).IsEmpty);

        Assert.IsFalse(
            inventory.TryGetItem(
                item.InstanceId,
                out _));
    }

    /// <summary>
    /// Verifies that removing an item from an empty slot fails and does
    /// not modify the inventory.
    /// </summary>
    [Test]
    public void Remove_RejectsEmptySlot()
    {
        // Arrange
        ItemDefinition definition =
            CreateDefinition("health_potion", 5);

        ItemDatabase database =
            CreateDatabase(definition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(inventory, database);

        // Act
        InventoryMutationResult result =
            mutation.Remove(
                0,
                out ItemInstance removedItem);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsNull(removedItem);

        Assert.IsTrue(
            inventory.TryGetSlot(0).IsEmpty);
    }

    /// <summary>
    /// Verifies that swapping two occupied slots exchanges the complete
    /// ItemInstances while preserving their identities and quantities.
    /// </summary>
    [Test]
    public void Swap_ExchangesItems()
    {
        // Arrange
        ItemDefinition potionDefinition =
            CreateDefinition("health_potion", 5);

        ItemDefinition swordDefinition =
            CreateDefinition("sword", 1);

        ItemDatabase database =
            CreateDatabase(
                potionDefinition,
                swordDefinition);

        Inventory inventory =
            new Inventory(5);

        InventoryMutationService mutation =
            new InventoryMutationService(
                inventory,
                database);

        ItemInstance potion =
            new ItemInstance("health_potion", 3);

        ItemInstance sword =
            new ItemInstance("sword", 1);

        Assert.IsTrue(
            mutation.Add(potion, out int potionSlot).Success);

        Assert.IsTrue(
            mutation.Add(sword, out int swordSlot).Success);

        // Act
        InventoryMutationResult result =
            mutation.Swap(potionSlot, swordSlot);

        // Assert
        Assert.IsTrue(result.Success);

        Assert.AreSame(
            sword,
            inventory.TryGetSlot(potionSlot).Item);

        Assert.AreSame(
            potion,
            inventory.TryGetSlot(swordSlot).Item);

        Assert.AreEqual(
            3,
            inventory.TryGetSlot(swordSlot).Item.Quantity);
    }

    // -------------------------------------------------------------------------
    // Test helpers
    // -------------------------------------------------------------------------

    static ItemDefinition CreateDefinition(
        string id,
        int maxStackSize)
    {
        ItemDefinition definition =
            ScriptableObject.CreateInstance<ItemDefinition>();

        definition.SetIdForTesting(id);
        definition.SetStackSize(maxStackSize);

        return definition;
    }

    static ItemDatabase CreateDatabase(
        params ItemDefinition[] definitions)
    {
        ItemDatabase database =
            ScriptableObject.CreateInstance<ItemDatabase>();

        database.SetDefinitionsForTesting(definitions);
        database.Initialize();

        return database;
    }
}
#endif