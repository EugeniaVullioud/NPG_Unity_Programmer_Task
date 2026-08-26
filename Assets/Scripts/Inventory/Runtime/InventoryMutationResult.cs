namespace Game.Inventory
{
    /// <summary>
    /// Represents the outcome of an inventory mutation.
    /// </summary>
    public readonly struct InventoryMutationResult
    {
        /// <summary>
        /// Gets whether the mutation succeeded.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the reason for failure when unsuccessful.
        /// </summary>
        public InventoryMutationFailure Failure { get; }

        private InventoryMutationResult(bool success, InventoryMutationFailure failure)
        {
            Success = success;
            Failure = failure;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        public static InventoryMutationResult Succeeded()
        {
            return new InventoryMutationResult(true, InventoryMutationFailure.None);
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        public static InventoryMutationResult Failed(InventoryMutationFailure failure)
        {
            return new InventoryMutationResult(false, failure);
        }
    }

    /// <summary>
    /// Identifies why an inventory mutation failed.
    /// </summary>
    public enum InventoryMutationFailure
    {
        None,
        InvalidSlot,
        EmptySlot,
        OccupiedSlot,
        ItemAlreadyInInventory,
        ItemNotFound,
        InvalidQuantity,
        NotStackable,
        StackFull,
        IncompatibleStack,
        SameSlot,
        InsufficientQuantity
    }
}

