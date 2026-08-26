using Game.SaveSystem;
namespace Game.Inventory
{
    /// <summary>
    /// Bridges the inventory domain with the global save system.
    /// This class is responsible only for converting inventory runtime state to and from persistent data. 
    /// It does not own save files, slots or filesystem operations.
    /// </summary>
    public sealed class InventorySaveParticipant : ISaveParticipant
    {
        /// <summary>
        /// Captures the current inventory state.
        /// </summary>
        public void Capture(SaveData saveData)
        {
        
        }

        /// <summary>
        /// Restores inventory state from persistent data.
        /// </summary>
        public void Restore(SaveData saveData)
        {

        }
    }
}