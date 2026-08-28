using Game.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game.SaveSystem
{
    public enum SaveLoadMode
    {
        Save,
        Load
    }
    /// <summary>
    /// Displays and manages the save/load UI and its associated save slots.
    /// </summary>
    public sealed class SaveLoadUI : BaseUI
    {

        /// <summary>
        /// Refreshes the save slot UI using the latest save slot information.
        /// </summary>
        public void Refresh()
        {
        }

        void OnSlotSelected(int slotId)
        {
            // Determine whether this means:
            // Load
            // Save
            // Delete
            // New save
            //
            // Confirmation can happen here at the UI/application boundary.
        }

    }
}