using Game.Inventory;
using Game.SaveSystem;
using UnityEngine;
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

        SaveLoadUI loadUI;
        BaseUI current;
        BaseUI last;

        void Awake()
        {
            if (saveLoad!= null) loadUI= saveLoad as SaveLoadUI;
        }
        /// <summary>
        /// Opens the inventory UI.
        /// </summary>
        public void OpenInventory()
        {
            last = current;
            current?.Close();
            inventory.Open();
            current = inventory;
        }

        /// <summary>
        /// Opens the save/load UI in save mode.
        /// </summary>
        public void OpenSave()
        {
            last = current;
            current?.Close();
            saveLoad.Open();
            loadUI.ShowSaveMode();
            current = saveLoad;

        }

        /// <summary>
        /// Opens the save/load UI in load mode.
        /// </summary>
        public void OpenLoad()
        {
            last = current;
            current?.Close();
            saveLoad.Open();
            loadUI.ShowLoadMode();
            current = saveLoad;
        }

        /// <summary>
        /// Opens the instructions UI.
        /// </summary>
        public void OpenInstructions()
        {
            last = current;
            current?.Close();
            instructions.Open();
            current = instructions;
        }

        /// <summary>
        /// Opens the pause UI.
        /// </summary>
        public void OpenPause()
        {
            last = current;
            current?.Close();
            pause.Open();
            current = pause;
        }

        public void Back()
        {
            current?.Close();
            last.Open();
            current = last;
        }
        public void SetCurrent(BaseUI ui)
        {
            last?.Close();
            last = current;
            current = ui;
        }
    }

}