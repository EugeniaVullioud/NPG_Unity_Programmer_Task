using Game.Inventory;
using Game.SaveSystem;
using System.Collections.Generic;
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
        readonly Stack<BaseUI> history = new();

        void Awake()
        {
            if (saveLoad != null) loadUI = saveLoad as SaveLoadUI;
        }

        public void OpenInventory()
        {
            Open(inventory);
        }

        public void OpenSave()
        {
            Open(saveLoad);
            loadUI?.ShowSaveMode();
        }

        public void OpenLoad()
        {
            Open(saveLoad);
            loadUI?.ShowLoadMode();
        }

        public void OpenInstructions()
        {
            Open(instructions);
        }

        public void OpenPause()
        {
            Open(pause);
        }

        void Open(BaseUI ui)
        {
            if (ui == null) return;

            if (current == ui) return;

            if (current != null)
            {
                current.Close();
                history.Push(current);
            }

            ui.Open();
            current = ui;
        }

        public void Back()
        {
            if (current == null) return;

            current.Close();

            if (history.Count == 0)
            {
                current = null;
                return;
            }

            current = history.Pop();
            current.Open();
        }

        public void SetCurrent(BaseUI ui)
        {
            if (ui == null) return;

            if (current == ui) return;

            if (current != null)
            {
                current.Close();
                history.Push(current);
            }

            current = ui;
        }
    }

}