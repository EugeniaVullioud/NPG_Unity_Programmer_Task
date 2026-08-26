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

        /// <summary>
        /// Creates a save manager using Unity's persistent data directory.
        /// </summary>
        public SaveManager(string directoryName = "Saves")
        {
            rootPath = Path.Combine(Application.persistentDataPath, directoryName);
            Directory.CreateDirectory(rootPath);
        }
        /// <summary>
        /// Registers a save participant.
        /// </summary>
        public void Register(ISaveParticipant participant)
        {
            if (participant == null) throw new ArgumentNullException(nameof(participant));
            if (string.IsNullOrWhiteSpace(participant.Id))
            {
                throw new ArgumentException("Participant ID cannot be empty.");
            }
            if (!participants.TryAdd(participant.Id, participant))
            {
                throw new InvalidOperationException($"Save participant '{participant.Id}' is already registered.");
            }
        }

        /// <summary>
        /// Saves all registered systems to a save slot.
        /// </summary>
        public void Save(string slotId)
        {
            if (string.IsNullOrWhiteSpace(slotId))
            {
                throw new ArgumentException("Save slot cannot be empty.", nameof(slotId));
            }

            SaveData saveData = SaveData.Create();

            saveData.Version = CurrentVersion;

            foreach (ISaveParticipant participant in participants.Values)
            {
                participant.Capture(saveData);
            }

            string json = JsonUtility.ToJson(saveData, false);

            string path = GetSlotPath(slotId);

            WriteAtomic(path, json);
        }

        /// <summary>
        /// Loads all registered systems from a save slot.
        /// </summary>
        public bool Load(string slotId)
        {
            string path = GetSlotPath(slotId);
            if (!File.Exists(path)) return false;

            try
            {
                string json = File.ReadAllText(path);

                SaveData saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData == null || saveData.Version > CurrentVersion) return false;

                foreach (ISaveParticipant participant in participants.Values)
                {
                    participant.Restore(saveData);
                }

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to load save slot {slotId}: {exception}");
                return false;
            }
        }

        /// <summary>
        /// Determines whether a save slot exists.
        /// </summary>
        public bool Exists(string slotId)
        {
            return File.Exists(GetSlotPath(slotId));
        }

        string GetSlotPath(string slotId)
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
