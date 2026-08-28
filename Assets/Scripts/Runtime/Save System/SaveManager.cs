using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using UnityEngine;

namespace Game.SaveSystem
{
    /// <summary>
    /// Coordinates saving and loading of complete game state.
    /// SaveManager is deliberately unaware of individual gameplay systems.
    /// Systems register themselves as ISaveParticipant implementations and provide their own serialization logic.
    /// </summary>
    public sealed class SaveManager
    {
        const int CurrentVersion = 1;

        readonly Dictionary<string, ISaveParticipant> participants = new(StringComparer.Ordinal);

        readonly string rootPath;

        public SaveManager(string directoryName = "Saves")
        {
            if (string.IsNullOrWhiteSpace(directoryName))
            {
                throw new ArgumentException("Save directory name cannot be empty.", nameof(directoryName));
            }
            rootPath = Path.Combine(Application.persistentDataPath, directoryName);
            Directory.CreateDirectory(rootPath);
        }

        public void Register(ISaveParticipant participant)
        {
            if (participant == null) throw new ArgumentNullException(nameof(participant));
            if (string.IsNullOrWhiteSpace(participant.Id))
            {
                throw new ArgumentException("Participant ID cannot be empty.");
            }
            if (!participants.TryAdd(participant.Id, participant))
            {
              //  throw new InvalidOperationException($"Save participant '{participant.Id}' is already registered.");
            }
        }

        /// <summary>
        /// Saves all registered systems to a save slot.
        /// </summary>
        public SaveOperationResult Save(string slotId)
        {
            if (!TryValidateSlotId(slotId, out SaveOperationResult validationResult))
            {
                return validationResult;
            }

            try
            {
                SaveData saveData = SaveData.Create();
                saveData.Version = CurrentVersion;

                PopulateMetadata(saveData);

                foreach (ISaveParticipant participant in participants.Values)
                {
                    participant.Capture(saveData);
                }

                string json = JsonUtility.ToJson(saveData, false);
             
                string path = GetSlotPath(slotId);

                if (!WriteAtomic(path, json))
                {
                    return SaveOperationResult.Failed(SaveOperationFailureReason.WriteFailed, $"Failed to write save slot '{slotId}'.");
                }

                return SaveOperationResult.Succeeded();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to save slot '{slotId}': {exception}");

                return SaveOperationResult.Failed(SaveOperationFailureReason.WriteFailed, exception.Message);
            }
        }
        void PopulateMetadata(SaveData saveData)
        {
            saveData.Metadata.DisplayName = "Save Game";

            saveData.Metadata.SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            saveData.Metadata.LastModifiedTicks = DateTime.UtcNow.Ticks;

            saveData.Metadata.PlaytimeSeconds = 0f;
        }
        /// <summary>
        /// Loads all registered systems from a save slot.
        /// </summary>
        public SaveOperationResult Load(string slotId)
        {
            if (!TryValidateSlotId(slotId, out SaveOperationResult validationResult))
            {
                return validationResult;
            }

            string path = GetSlotPath(slotId);

            if (!File.Exists(path))
            {
                return SaveOperationResult.Failed(SaveOperationFailureReason.SlotNotFound, $"Save slot '{slotId}' does not exist.");
            }

            try
            {
                string json = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return SaveOperationResult.Failed(SaveOperationFailureReason.InvalidData, $"Save slot '{slotId}' is empty.");
                }

                SaveData saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData == null)
                {
                    return SaveOperationResult.Failed(SaveOperationFailureReason.InvalidData, $"Save slot '{slotId}' contains invalid save data.");
                }

                if (saveData.Version > CurrentVersion)
                {
                    return SaveOperationResult.Failed(SaveOperationFailureReason.InvalidData, $"Save slot '{slotId}' uses unsupported save version {saveData.Version}.");
                }

                foreach (ISaveParticipant participant in participants.Values)
                {
                    if (!participant.Restore(saveData))
                    {
                        return SaveOperationResult.Failed(SaveOperationFailureReason.InvalidData, $"Participant '{participant.Id}' failed to restore.");
                    }
                }

                return SaveOperationResult.Succeeded();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to load save slot '{slotId}': {exception}");

                return SaveOperationResult.Failed(SaveOperationFailureReason.ReadFailed, exception.Message);
            }
        }
        /// <summary>
        /// Deletes all save files from the save directory.
        /// </summary>
        public SaveOperationResult DeleteAll()
        {
            try
            {
                if (!Directory.Exists(rootPath))
                {
                    return SaveOperationResult.Succeeded();
                }

                Directory.Delete(rootPath, true);

                //foreach (string file in Directory.GetFiles(rootPath)) File.Delete(file);

                Directory.CreateDirectory(rootPath);

                return SaveOperationResult.Succeeded();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to delete all saves: {exception}");

                return SaveOperationResult.Failed(SaveOperationFailureReason.WriteFailed, exception.Message);
            }
        }
        /// <summary>
        /// Deletes a save slot.
        /// </summary>
        public SaveOperationResult Delete(string slotId)
        {
            if (!TryValidateSlotId(slotId, out SaveOperationResult validationResult))
            {
                return validationResult;
            }

            string path = GetSlotPath(slotId);

            if (!File.Exists(path))
            {
                return SaveOperationResult.Failed(SaveOperationFailureReason.SlotNotFound, $"Save slot '{slotId}' does not exist.");
            }

            try
            {
                File.Delete(path);

                return SaveOperationResult.Succeeded();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to delete save slot '{slotId}': {exception}");

                return SaveOperationResult.Failed(SaveOperationFailureReason.WriteFailed, exception.Message);
            }
        }

        /// <summary>
        /// Determines whether a save slot exists.
        /// </summary>
        public bool Exists(string slotId)
        {
            if (string.IsNullOrWhiteSpace(slotId)) return false;
            return File.Exists(GetSlotPath(slotId));
        }

        /// <summary>
        /// Gets the last modification time of a save slot.
        /// </summary>
        public bool TryGetLastModified(string slotId, out DateTime lastModified)
        {
            lastModified = default;

            if (!Exists(slotId)) return false;

            try
            {
                lastModified = File.GetLastWriteTime(GetSlotPath(slotId));

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to read modification time for save slot '{slotId}': {exception}");

                return false;
            }
        }

        bool TryValidateSlotId(string slotId, out SaveOperationResult result)
        {
            if (string.IsNullOrWhiteSpace(slotId))
            {
                result = SaveOperationResult.Failed(SaveOperationFailureReason.InvalidOperation, "Save slot cannot be empty.");
                return false;
            }

            result = default;
            return true;
        }

        public bool TryReadSaveData(string path, out SaveData saveData)
        {
            saveData = null;

            try
            {
                string json = File.ReadAllText(path);

                saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData == null || saveData.Version > CurrentVersion) return false;

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to read save file '{path}': {exception}");

                return false;
            }
        }
        public string GetSlotPath(string slotId)
        {
            return Path.Combine(rootPath, SanitizeSlotId(slotId) + ".json");
        }
        /// <summary>
        /// Replaces characters that are invalid in file names with underscores.
        /// </summary>
        static string SanitizeSlotId(string slotId)
        {
            foreach (char invalid in Path.GetInvalidFileNameChars())
            {
                slotId = slotId.Replace(invalid, '_');
            }
            return slotId;
        }
        /// <summary>
        /// Writes contents to a file using a temporary file and backup to reduce the risk of losing the existing save if the write fails.
        /// </summary>
        static bool WriteAtomic(string path, string contents)
        {
            string tempPath = path + ".tmp";
            string backupPath = path + ".backup";

            try
            {
                File.WriteAllText(tempPath, contents);

                if (File.Exists(path))
                {
                    File.Copy(path, backupPath, true);
                    File.Delete(path);
                }

                File.Move(tempPath, path);
                if (!File.Exists(path))
                {
                    throw new IOException($"File move completed but destination does not exist: '{path}'");
                }
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to write save file '{path}': {exception}");

                try
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
                catch
                {
                    // Deliberately ignored during recovery.
                }
                return false;
            }
        }
    }
}
