using System;
using System.Collections.Generic;
using System.IO;

namespace Game.SaveSystem
{
    /// <summary>
    /// Provides a UI/application-facing API for save game operations.
    /// </summary>
    /// <remarks>
    /// This service translates application requests into operations against the
    /// existing <see cref="SaveManager"/>.
    /// </remarks>
    public sealed class SaveGameService
    {
        readonly SaveManager saveManager;
        readonly int slotCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveGameService"/> class.
        /// </summary>
        /// <param name="saveManager">
        /// The save manager used to perform the underlying save operations.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="saveManager"/> is <c>null</c>.
        /// </exception>
        public SaveGameService(SaveManager saveManager, int slotCount)
        {
            this.saveManager = saveManager ?? throw new ArgumentNullException(nameof(saveManager));
            if (slotCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slotCount));
            }
            this.slotCount = slotCount;
        }

        /// <summary>
        /// Gets information about the available save slots.
        /// </summary>
        /// <returns>
        /// A read-only list containing information about each save slot.
        /// </returns>
        public IReadOnlyList<SaveSlotInfo> GetSlots()
        {
            List<SaveSlotInfo> result = new List<SaveSlotInfo>(slotCount);
            
            for (int i = 0; i < slotCount; i++)
            {
                int slotId = i + 1;

                bool exists = saveManager.Exists(slotId.ToString());

                if(! exists)
                {
                    result.Add(new SaveSlotInfo(slotId, false, string.Empty, string.Empty, default, TimeSpan.Zero));

                    continue;
                }
                // Exists
                string path = saveManager.GetSlotPath(slotId.ToString());

                if (!saveManager.TryReadSaveData(path, out SaveData saveData))
                {
                    // The slot exists, but its metadata could not be read.
                    result.Add(new SaveSlotInfo(slotId, true, $"Slot {slotId}", string.Empty, File.GetLastWriteTimeUtc(path), TimeSpan.Zero));
                    continue;
                }
                SaveMetadata metadata = saveData.Metadata;

                if (metadata == null)
                {
                    result.Add(new SaveSlotInfo(slotId, true, $"Slot {slotId}", string.Empty,
                            File.GetLastWriteTimeUtc(path), TimeSpan.Zero));

                    continue;
                }

                result.Add(new SaveSlotInfo(slotId, true, metadata.DisplayName, metadata.SceneName, metadata.LastModified, metadata.Playtime));


                /* prev
                saveManager.TryGetLastModified(slotId.ToString(), out lastModified);
                result.Add(new SaveSlotInfo(slotId, exists, $"Slot {slotId}", string.Empty, lastModified, TimeSpan.Zero));*/
            }
            return result;
        }

        /// <summary>
        /// Saves the current game state to the specified save slot.
        /// </summary>
        /// <param name="slotId">The identifier of the save slot.</param>
        /// <returns>
        /// The result of the save operation.
        /// </returns>
        public SaveOperationResult Save(int slotId)
        {
            if (!IsConfiguredSlot(slotId))
            {
                return SaveOperationResult.Failed(SaveOperationFailureReason.InvalidOperation, $"Save slot {slotId} is not configured.");
            }

            return saveManager.Save(slotId.ToString());
        }


        /// <summary>
        /// Loads the game state from the specified save slot.
        /// </summary>
        /// <param name="slotId">The identifier of the save slot.</param>
        /// <returns>
        /// The result of the load operation.
        /// </returns>
        public SaveOperationResult Load(int slotId)
        {
            if (!IsConfiguredSlot(slotId))
            {
                return SaveOperationResult.Failed(SaveOperationFailureReason.InvalidOperation, $"Save slot {slotId} is not configured.");
            }

            return saveManager.Load(slotId.ToString());
        }
        /// <summary>
        /// Deletes the save data from the specified save slot.
        /// </summary>
        public SaveOperationResult Delete(int slotId)
        {
            if (!IsConfiguredSlot(slotId))
            {
                return SaveOperationResult.Failed(SaveOperationFailureReason.InvalidOperation, $"Save slot {slotId} is not configured.");
            }
            return saveManager.Delete(slotId.ToString());
        }

        bool IsConfiguredSlot(int slotId)
        {
            return slotId >= 1 && slotId <= slotCount;
        }
    }
}