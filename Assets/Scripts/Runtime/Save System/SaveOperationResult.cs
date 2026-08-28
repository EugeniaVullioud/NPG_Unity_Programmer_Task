using System;
namespace Game.SaveSystem
{
    /// <summary>
    /// Describes the outcome of an application-level save operation.
    /// </summary>
    public readonly struct SaveOperationResult
    {
        /// <summary>
        /// Gets whether the operation completed successfully.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the reason the operation failed.
        /// </summary>
        public SaveOperationFailureReason FailureReason { get; }

        /// <summary>
        /// Gets an optional user-facing or diagnostic message.
        /// </summary>
        public string Message { get; }

        private SaveOperationResult(bool success, SaveOperationFailureReason failureReason, string message)
        {
            Success = success;
            FailureReason = failureReason;
            Message = message;
        }

        /// <summary>
        /// Creates a successful operation result.
        /// </summary>
        public static SaveOperationResult Succeeded(string message = null)
        {
            return new SaveOperationResult(success: true, failureReason: SaveOperationFailureReason.None, message: message);
        }

        /// <summary>
        /// Creates a failed operation result.
        /// </summary>
        public static SaveOperationResult Failed(SaveOperationFailureReason reason, string message = null)
        {
            if (reason == SaveOperationFailureReason.None)
            {
                throw new ArgumentException("A failed save operation must specify a failure reason.", nameof(reason));
            }

            return new SaveOperationResult(success: false, failureReason: reason, message: message);
        }
    }
    /// <summary>
    /// Describes why a save-related application operation failed.
    /// </summary>
    public enum SaveOperationFailureReason
    {
        None,
        SlotNotFound,
        SlotOccupied,
        SlotEmpty,

        /// <summary>
        /// The save data could not be written.
        /// </summary>
        WriteFailed,

        /// <summary>
        /// The save data could not be read.
        /// </summary>
        ReadFailed,

        /// <summary>
        /// The save data was invalid or corrupted.
        /// </summary>
        InvalidData,

        /// <summary>
        /// The requested operation is not currently valid.
        /// </summary>
        InvalidOperation
    }
}