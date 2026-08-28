using System;

namespace Game.SaveSystem
{
    /// <summary>
    /// Metadata associated with a saved game.
    /// This data is persisted alongside the gameplay state so save-slot
    /// presentation can be reconstructed without loading the full game.
    /// </summary>
    [Serializable]
    public sealed class SaveMetadata
    {
        /// <summary>
        /// Display name shown by the save/load UI.
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Name of the scene associated with the save.
        /// </summary>
        public string SceneName;

        /// <summary>
        /// UTC timestamp represented as DateTime ticks.
        /// </summary>
        public long LastModifiedTicks;

        /// <summary>
        /// Total playtime represented in seconds.
        /// </summary>
        public float PlaytimeSeconds;

        public DateTime LastModified => new DateTime(LastModifiedTicks, DateTimeKind.Utc);

        public TimeSpan Playtime => TimeSpan.FromSeconds(PlaytimeSeconds);
    }
}