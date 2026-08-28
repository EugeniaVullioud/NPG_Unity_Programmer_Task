using Game.Inventory;
using Game.SaveSystem;
using UnityEngine;
using static Codice.CM.Common.CmCallContext;
namespace Game.UI
{
    /// <summary>
    /// Coordinates navigation between the primary gameplay UI panels.
    /// </summary>
    /// <remarks>
    /// <see cref="GameplayUI"/> acts as a UI coordinator and delegates  presentation and behavior to the individual UI components.
    /// </remarks>
    public sealed class GameplayUI : MonoBehaviour
    {
        [SerializeField] BaseUI inventory;
        [SerializeField] BaseUI saveLoad;
        [SerializeField] BaseUI instructions;
        [SerializeField] BaseUI pause;

        BaseUI current;

        /// <summary>
        /// Opens the inventory UI.
        /// </summary>
        public void OpenInventory()
        {
            current?.Close();
            inventory.Open();
            current = inventory;
        }

        /// <summary>
        /// Opens the save/load UI in save mode.
        /// </summary>
        public void OpenSave()
        {
            current?.Close();

            //saveLoad.ShowSave();
            current = saveLoad;

        }

        /// <summary>
        /// Opens the save/load UI in load mode.
        /// </summary>
        public void OpenLoad()
        {
            current?.Close();

            //saveLoad.ShowLoad();
            current = saveLoad;

        }

        /// <summary>
        /// Opens the instructions UI.
        /// </summary>
        public void OpenInstructions()
        {
            current?.Close();
            instructions.Open();
            current = instructions;
        }

        /// <summary>
        /// Opens the pause UI.
        /// </summary>
        public void OpenPause()
        {
            current?.Close();
            pause.Open();
            current = pause;
        }
    }

}