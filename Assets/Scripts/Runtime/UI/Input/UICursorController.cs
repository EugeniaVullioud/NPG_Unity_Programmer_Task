using System.Collections.Generic;
using UnityEngine;
namespace Game.UI
{
    /// <summary>
    /// Controls the visibility and lock state of the application cursor based on the currently active UI input sources.
    /// </summary>
    public sealed class UICursorController: MonoBehaviour
    {
        readonly HashSet<UIInputSource> activeSources = new();

        /// <summary>
        /// Registers a UI input source as requiring pointer interaction.
        /// </summary>
        public void Register(UIInputSource source)
        {
            if (source == null) return;

            activeSources.Add(source);
            Refresh();
        }

        /// <summary>
        /// Unregisters a UI input source and refreshes the cursor state.
        /// </summary>
        public void Unregister(UIInputSource source)
        {
            if (source == null) return;

            activeSources.Remove(source);
            Refresh();
        }
        /// <summary>
        /// Updates the cursor visibility and lock state based on whether any
        /// registered UI input sources are active.
        void Refresh()
        {
            bool requiresPointer = activeSources.Count > 0;

            Cursor.visible = requiresPointer;
            Cursor.lockState = requiresPointer ? CursorLockMode.None : CursorLockMode.Locked;
        }
        /// <summary>
        /// Clears all registered UI input sources and restores the cursor to
        /// its gameplay state when this controller is disabled.
        /// </summary>
        void OnDisable()
        {
            activeSources.Clear();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}