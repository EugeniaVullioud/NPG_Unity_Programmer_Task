using System;

namespace Game.Inventory
{
    /// <summary>
    /// Performs validated mutations against an authoritative inventory.
    /// This class owns operation orchestration while Inventory owns state invariants.
    /// </summary>
    public sealed class InventoryMutationService
    {
        readonly Inventory inventory;
        readonly ItemDatabase itemDatabase;

        /// <summary>
        /// Creates an inventory mutation service.
        /// </summary>
        public InventoryMutationService(Inventory inventory, ItemDatabase itemDatabase)
        {
            this.inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));

            this.itemDatabase = itemDatabase ?? throw new ArgumentNullException(nameof(itemDatabase));
        }

        /// <summary>
        /// Attempts to add an item to the inventory.
        /// Existing compatible stacks are filled before an empty slot is used.
        /// </summary>
        public InventoryMutationResult Add(ItemInstance item, out int resultingSlot)
        {
            resultingSlot = -1;

            if (item == null)
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.ItemNotFound);
            }

            if (!itemDatabase.TryGet(item.DefinitionId, out ItemDefinition definition))
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.ItemNotFound);
            }

            if (inventory.TryGetItem(item.InstanceId, out _))
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.ItemAlreadyInInventory);
            }

            int remaining = item.Quantity;

            if (definition.MaxStackSize > 1)
            {
                for (int i = 0; i < inventory.Capacity; i++)
                {
                    InventorySlot slot = inventory.TryGetSlot(i);

                    if (slot == null || slot.IsEmpty) continue;

                    ItemInstance existing = slot.Item;

                    if (!CanStack(existing, item, definition)) continue;

                    int available = definition.MaxStackSize - existing.Quantity;

                    if (available <= 0) continue;

                    int amount = Math.Min(available, remaining);

                    existing.SetQuantity(existing.Quantity + amount);

                    remaining -= amount;

                    if (remaining == 0)
                    {
                        return InventoryMutationResult.Succeeded();
                    }
                }
            }

            while (remaining > 0)
            {
                int emptySlot = FindEmptySlot();

                if (emptySlot < 0)
                {
                    return InventoryMutationResult.Failed(InventoryMutationFailure.StackFull);
                }

                int amount = Math.Min(remaining, definition.MaxStackSize);

                ItemInstance newStack;

                if (remaining == item.Quantity)
                {
                    newStack = item;

                    if (amount != item.Quantity)
                    {
                        newStack = new ItemInstance(item.DefinitionId, amount);

                        CopyRuntimeState(item, newStack);
                    }
                }
                else
                {
                    newStack = new ItemInstance(item.DefinitionId, amount);
                }

                if (!inventory.TryPlace(emptySlot, newStack))
                {
                    return InventoryMutationResult.Failed(InventoryMutationFailure.StackFull);
                }

                resultingSlot = emptySlot;
                remaining -= amount;
            }
            return InventoryMutationResult.Succeeded();
        }

        /// <summary>
        /// Removes an item from a slot and returns the removed instance.
        /// </summary>
        public InventoryMutationResult Remove(int slotIndex, out ItemInstance removedItem)
        {
            removedItem = null;

            if (!IsValidSlot(slotIndex))
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.InvalidSlot);
            }

            InventorySlot slot = inventory.TryGetSlot(slotIndex);

            if (slot.IsEmpty)
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.EmptySlot);
            }

            removedItem = inventory.RemoveAt(slotIndex);

            return InventoryMutationResult.Succeeded();
        }

        /// <summary>
        /// Moves an item into an empty destination slot.
        /// </summary>
        public InventoryMutationResult Move(int sourceIndex, int destinationIndex)
        {
            if (!IsValidSlot(sourceIndex) || !IsValidSlot(destinationIndex))
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.InvalidSlot);
            }

            if (sourceIndex == destinationIndex)
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.SameSlot);
            }

            InventorySlot source = inventory.TryGetSlot(sourceIndex);
            InventorySlot destination = inventory.TryGetSlot(destinationIndex);

            if (source.IsEmpty)
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.EmptySlot);
            }
            if (destination.IsEmpty)
            {
                return inventory.Move(sourceIndex, destinationIndex)
                 ? InventoryMutationResult.Succeeded()
                 : InventoryMutationResult.Failed(InventoryMutationFailure.OccupiedSlot);
            }
            // Destination is occupied

            if (!itemDatabase.TryGet(destination.Item.DefinitionId, out ItemDefinition definition))
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.ItemNotFound);
            }

            // Destination contains an incompatible item.
            if (!CanStack(source.Item, destination.Item, definition))
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.OccupiedSlot);
            }

            // Merge compatible stacks.

            return MergeStacks(sourceIndex, source.Item, destination.Item, definition);
        }
        InventoryMutationResult MergeStacks(int sourceIndex, ItemInstance source, ItemInstance destination, ItemDefinition definition)
        {
            int available =                definition.MaxStackSize - destination.Quantity;

            if (available <= 0)
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.StackFull);
            }

            int amount = Math.Min(available, source.Quantity);

            int remaining = source.Quantity - amount;

            destination.SetQuantity(destination.Quantity + amount);

            if (remaining == 0)
            {
                inventory.RemoveAt(sourceIndex);
            }
            else
            {
                source.SetQuantity(remaining);
            }

            return InventoryMutationResult.Succeeded();
        }
        /// <summary>
        /// Swaps two inventory slots atomically.
        /// </summary>
        public InventoryMutationResult Swap(int firstIndex, int secondIndex)
        {
            if (!IsValidSlot(firstIndex) || !IsValidSlot(secondIndex))
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.InvalidSlot);
            }

            if (firstIndex == secondIndex)
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.SameSlot);
            }

            return inventory.Swap(firstIndex, secondIndex)
                ? InventoryMutationResult.Succeeded()
                : InventoryMutationResult.Failed(InventoryMutationFailure.EmptySlot);
        }

        /// <summary>
        /// Splits part of a stack into an empty destination slot.
        /// </summary>
        public InventoryMutationResult Split(int sourceIndex, int destinationIndex, int amount)
        {
            if (!IsValidSlot(sourceIndex) || !IsValidSlot(destinationIndex))
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.InvalidSlot);
            }

            if (amount <= 0)
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.InvalidQuantity);
            }

            InventorySlot source = inventory.TryGetSlot(sourceIndex);

            InventorySlot destination = inventory.TryGetSlot(destinationIndex);

            if (source.IsEmpty)
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.EmptySlot);
            }
            if (!destination.IsEmpty)
            {
                // Should check if same type of item before defaulting to failure.
                return InventoryMutationResult.Failed(InventoryMutationFailure.OccupiedSlot);
            }

            ItemInstance sourceItem = source.Item;

            if (amount >= sourceItem.Quantity)
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.InsufficientQuantity);
            }

            if (!itemDatabase.TryGet(sourceItem.DefinitionId, out ItemDefinition definition))
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.ItemNotFound);
            }

            if (definition.MaxStackSize <= 1)
            {
                return InventoryMutationResult.Failed(InventoryMutationFailure.NotStackable);
            }

            sourceItem.SetQuantity(sourceItem.Quantity - amount);

            ItemInstance splitItem = new ItemInstance(sourceItem.DefinitionId, amount);

            CopyRuntimeState(sourceItem, splitItem);

            if (!inventory.TryPlace(destinationIndex, splitItem))
            {
                sourceItem.SetQuantity(sourceItem.Quantity + amount);

                return InventoryMutationResult.Failed(InventoryMutationFailure.OccupiedSlot);
            }
            return InventoryMutationResult.Succeeded();
        }

        bool CanStack(ItemInstance left, ItemInstance right, ItemDefinition definition)
        {
            if (left == null || right == null) return false;
            if (left.DefinitionId != right.DefinitionId) return false;
            if (left.Quantity >= definition.MaxStackSize) return false;

            if (!left.Durability.Equals(right.Durability)) return false;
            if (left.Modifiers.Count != right.Modifiers.Count) return false;

            for (int i = 0; i < left.Modifiers.Count; i++)
            {
                ItemModifierInstance a = left.Modifiers[i];
                ItemModifierInstance b = right.Modifiers[i];

                if (a.DefinitionId != b.DefinitionId || !a.Value.Equals(b.Value)) return false;

            }
            return true;
        }

        static void CopyRuntimeState(ItemInstance source, ItemInstance destination)
        {
            destination.SetDurability(source.Durability);

            for (int i = 0; i < source.Modifiers.Count; i++)
            {
                ItemModifierInstance sourceModifier = source.Modifiers[i];

                destination.AddModifier(new ItemModifierInstance(sourceModifier.DefinitionId, sourceModifier.Value));
            }
        }

        int FindEmptySlot()
        {
            for (int i = 0; i < inventory.Capacity; i++)
            {
                if (inventory.TryGetSlot(i).IsEmpty) return i;

            }
            return -1;
        }

        bool IsValidSlot(int index)
        {
            return index >= 0 && index < inventory.Capacity;
        }
    }
}
