namespace Game.SaveSystem
{
    /// <summary>
    /// Defines a system capable of contributing state to and restoring state from the game's save data.
    /// </summary>
    public interface ISaveParticipant
    {
        /// <summary>
        /// Writes the current system state into the supplied save container.
        /// </summary>
        void Capture(SaveData saveData);

        /// <summary>
        /// Restores system state from the supplied save container.
        /// </summary>
        bool Restore(SaveData saveData);
    }
}
