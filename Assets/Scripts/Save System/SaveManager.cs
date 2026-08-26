using System;
using System.Collections.Generic;
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
        readonly List<ISaveParticipant> participants = new List<ISaveParticipant>();

        /// <summary>
        /// Registers a system with the save manager.
        /// </summary>
        public void Register(ISaveParticipant participant)
        {
            if (participant == null) throw new ArgumentNullException(nameof(participant));

            if (!participants.Contains(participant))
            {
                participants.Add(participant);
            }
        }

        /// <summary>
        /// Removes a system from the save manager.
        /// </summary>
        public void Unregister(ISaveParticipant participant)
        {
            participants.Remove(participant);
        }

        /// <summary>
        /// Saves the complete game state to the specified save slot.
        /// </summary>
        public void Save(int slot)
        {
            SaveData data = new SaveData();

            foreach (ISaveParticipant participant in participants)
            {
                participant.Capture(data);
            }

            string json = JsonUtility.ToJson(data, true);

            string path = GetPath(slot);
            string temporaryPath = path + ".tmp";

            File.WriteAllText(temporaryPath, json);

            if (File.Exists(path)) File.Delete(path);

            File.Move(temporaryPath, path);
        }

        /// <summary>
        /// Loads complete game state from the specified save slot.
        /// </summary>
        public bool Load(int slot)
        {
            string path = GetPath(slot);

            if (!File.Exists(path)) return false;

            try
            {
                string json = File.ReadAllText(path);

                SaveData data = JsonUtility.FromJson<SaveData>(json);

                if (data == null) return false;

                foreach (ISaveParticipant participant in participants)
                {
                    participant.Restore(data);
                }
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to load save slot {slot}: {exception}");

                return false;
            }
        }

        /// <summary>
        /// Determines whether a save slot exists.
        /// </summary>
        public bool Exists(int slot)
        {
            return File.Exists(GetPath(slot));
        }

        static string GetPath(int slot)
        {
            return Path.Combine(Application.persistentDataPath, $"save_{slot}.json");
        }
    }
}