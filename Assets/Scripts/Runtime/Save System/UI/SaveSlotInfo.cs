using System;

namespace Game.SaveSystem
{
    /// <summary>
    /// Contains metadata describing the state of a save slot.
    /// </summary>
    public readonly struct SaveSlotInfo
    {
        /// <summary>
        /// Gets the unique identifier of the save slot.
        /// </summary>
        public int SlotId { get; }

        /// <summary>
        /// Gets a value indicating whether the save slot contains save data.
        /// </summary>
        public bool Exists { get; }

        /// <summary>
        /// Gets the display name associated with the save slot.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the name of the scene associated with the save.
        /// </summary>
        public string SceneName { get; }

        /// <summary>
        /// Gets the date and time when the save was last modified.
        /// </summary>
        public DateTime LastModified { get; }

        /// <summary>
        /// Gets the total playtime recorded by the save.
        /// </summary>
        public TimeSpan Playtime { get; }

        public SaveSlotInfo(int slotId, bool exists, string displayName, string sceneName, DateTime lastModified, TimeSpan playtime)
        {
            SlotId = slotId;
            Exists = exists;
            DisplayName = displayName;
            SceneName = sceneName;
            LastModified = lastModified;
            Playtime = playtime;
        }
    }
}