using System;
using System.Collections.Generic;

namespace Game.Inventory
{
    /// <summary>
    /// Represents one mutable runtime instance of an item definition.
    /// Static configuration is resolved through ItemDefinition.
    /// </summary>
    public sealed class ItemInstance
    {
        readonly string instanceId;
        readonly string definitionId;

        int quantity;
        /// <summary>
        /// Raised whenever mutable item state changes.
        /// </summary>
        public event Action<ItemInstance> Changed;

        /// <summary>
        /// Gets the unique identifier of this item instance.
        /// </summary>
        public string InstanceId => instanceId;

        /// <summary>
        /// Gets the item definition identifier.
        /// </summary>
        public string DefinitionId => definitionId;

        public int Quantity => quantity;

        // <summary>
        /// Creates a new runtime item instance.
        /// </summary>
        public ItemInstance(string definitionId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(definitionId))
            {
                throw new ArgumentException("Definition ID cannot be empty.", nameof(definitionId));
            }
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity));
            }

            instanceId = Guid.NewGuid().ToString("N");
            this.definitionId = definitionId;
            this.quantity = quantity;
        }


        /// <summary>
        /// Restores a runtime item instance from persisted state.
        /// </summary>
        internal ItemInstance(string instanceId, string definitionId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(instanceId))
            {
                throw new ArgumentException("Instance ID cannot be empty.", nameof(instanceId));
            }
            if (string.IsNullOrWhiteSpace(definitionId))
            {
                throw new ArgumentException("Definition ID cannot be empty.", nameof(definitionId));
            }
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity));
            }

            this.instanceId = instanceId;
            this.definitionId = definitionId;
            this.quantity = quantity;
        }

        /// <summary>
        /// Changes the item quantity.
        /// </summary>
        public void SetQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity));
            }

            if (this.quantity == quantity) return;

            this.quantity = quantity;
            NotifyChanged();
        }
        /// <summary>
        /// Explicitly notifies subscribers that a batch operation changed the item.
        /// </summary>
        internal void NotifyChanged()
        {
            Changed?.Invoke(this);
        }

    }
}
