using System;
using System.Collections.Generic;

namespace Game.Inventory
{
    /// <summary>
    /// Inventory owns slot state, item membership and identity invariants.
    /// Complex gameplay operations belong to services such as InventoryMutationService.
    /// </summary>
    public sealed class Inventory
    {
        readonly InventorySlot[] slots;

        readonly Dictionary<string, ItemInstance> itemLookup = new(StringComparer.Ordinal);
        readonly Dictionary<string, int> itemSlotLookup = new(StringComparer.Ordinal);

        /// <summary>
        /// Raised whenever inventory structure or item state changes.
        /// </summary>
        public event Action<InventoryChangedEventArgs> Changed;

        /// <summary>
        /// Gets the number of inventory slots.
        /// </summary>
        public int Capacity => slots.Length;

        /// <summary>
        /// Creates an inventory with the specified capacity.
        /// </summary>
        public Inventory(int capacity)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));

            slots = new InventorySlot[capacity];

            for (int i = 0; i < capacity; i++)
            {
                slots[i] = new InventorySlot(i);
            }
        }

        /// <summary>
        /// Gets a slot without throwing for invalid indexes.
        /// </summary>
        public InventorySlot TryGetSlot(int index)
        {
            if (index < 0 || index >= slots.Length) return null;

            return slots[index];
        }

        /// <summary>
        /// Gets the fixed inventory slots.
        /// </summary>
        public IReadOnlyList<InventorySlot> Slots => slots;

        /// <summary>
        /// Attempts to locate an item by persistent instance ID.
        /// </summary>
        public bool TryGetItem(string instanceId, out ItemInstance item)
        {
            return itemLookup.TryGetValue(instanceId, out item);
        }

        /// <summary>
        /// Attempts to retrieve the slot containing an item.
        /// </summary>
        public bool TryGetItemSlot(string instanceId, out int slotIndex)
        {
            return itemSlotLookup.TryGetValue(instanceId, out slotIndex);
        }

        /// <summary>
        /// Returns whether the specified slot contains an item.
        /// </summary>
        public bool IsOccupied(int slotIndex)
        {
            ValidateSlotIndex(slotIndex);
            return !slots[slotIndex].IsEmpty;
        }

        internal bool TryPlace(int slotIndex, ItemInstance item)
        {
            ValidateSlotIndex(slotIndex);

            if (item == null || !slots[slotIndex].IsEmpty) return false;

            if (itemLookup.ContainsKey(item.InstanceId)) return false;

            RegisterItem(slotIndex, item);
            slots[slotIndex].SetItem(item);

            RaiseChanged(InventoryChangeType.Added, slotIndex, -1, item, null);

            return true;
        }

        internal ItemInstance RemoveAt(int slotIndex)
        {
            ValidateSlotIndex(slotIndex);

            InventorySlot slot = slots[slotIndex];
            if (slot.IsEmpty) return null;

            ItemInstance item = slot.Clear();

            UnregisterItem(item);

            RaiseChanged(InventoryChangeType.Removed, slotIndex, -1, null, item);

            return item;
        }

        internal bool Move(int sourceIndex, int destinationIndex)
        {
            ValidateSlotIndex(sourceIndex);
            ValidateSlotIndex(destinationIndex);

            if (sourceIndex == destinationIndex) return false;

            InventorySlot source = slots[sourceIndex];
            InventorySlot destination = slots[destinationIndex];

            if (source.IsEmpty || !destination.IsEmpty) return false;

            ItemInstance item = source.Clear();
            destination.SetItem(item);

            itemSlotLookup[item.InstanceId] = destinationIndex;

            RaiseChanged(InventoryChangeType.Moved, sourceIndex, destinationIndex, item, null);

            return true;
        }

        internal bool Swap(int firstIndex, int secondIndex)
        {
            ValidateSlotIndex(firstIndex);
            ValidateSlotIndex(secondIndex);

            if (firstIndex == secondIndex) return false;

            InventorySlot first = slots[firstIndex];
            InventorySlot second = slots[secondIndex];

            if (first.IsEmpty && second.IsEmpty) return false;

            ItemInstance firstItem = first.Item;
            ItemInstance secondItem = second.Item;

            first.SetItem(secondItem);
            second.SetItem(firstItem);

            if (firstItem != null)
            {
                itemSlotLookup[firstItem.InstanceId] = secondIndex;
            }

            if (secondItem != null)
            {
                itemSlotLookup[secondItem.InstanceId] = firstIndex;
            }

            RaiseChanged(InventoryChangeType.Swapped, firstIndex, secondIndex, secondItem, firstItem);

            return true;
        }

        internal void ClearWithoutEvents()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                InventorySlot slot = slots[i];

                if (slot.IsEmpty) continue;

                ItemInstance item = slot.Clear();
                UnregisterItem(item);
            }
        }

        internal void RestoreSlot(int slotIndex, ItemInstance item)
        {
            ValidateSlotIndex(slotIndex);

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (!slots[slotIndex].IsEmpty)
            {
                throw new InvalidOperationException($"Slot {slotIndex} is already occupied.");
            }
            if (itemLookup.ContainsKey(item.InstanceId))
            {
                throw new InvalidOperationException($"Duplicate item instance ID '{item.InstanceId}'.");
            }
            RegisterItem(slotIndex, item);
            slots[slotIndex].SetItem(item);
        }

        internal void RaiseLoaded()
        {
            RaiseChanged(InventoryChangeType.Cleared, -1, -1, null, null);
        }

        void RegisterItem(int slotIndex, ItemInstance item)
        {
            itemLookup.Add(item.InstanceId, item);
            itemSlotLookup.Add(item.InstanceId, slotIndex);

            item.Changed += OnItemChanged;
        }

        void UnregisterItem(ItemInstance item)
        {
            if (item == null) return;

            item.Changed -= OnItemChanged;

            itemLookup.Remove(item.InstanceId);
            itemSlotLookup.Remove(item.InstanceId);
        }

        void OnItemChanged(ItemInstance item)
        {
            if (!itemLookup.ContainsKey(item.InstanceId)) return;
            if (!itemSlotLookup.TryGetValue(item.InstanceId, out int slotIndex)) return;

            RaiseChanged(InventoryChangeType.ItemChanged, slotIndex, -1, item, null);
        }

        void RaiseChanged(InventoryChangeType changeType, int slotIndex, int secondarySlotIndex, ItemInstance item, ItemInstance previousItem)
        {
            Changed?.Invoke(new InventoryChangedEventArgs(changeType, slotIndex, secondarySlotIndex, item, previousItem));
        }

        void ValidateSlotIndex(int index)
        {
            if (index < 0 || index >= slots.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
}