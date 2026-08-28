using Game.Inventory;
using Game.SaveSystem;
using UnityEngine;
namespace Game.UI
{
    // <summary>
    /// Installs and initializes the UI dependencies required by the scene.
    /// </summary>
    public class SceneUIInstaller
    {
        InventoryUI inventoryUI;
        BaseUI instructionsUI;
        SaveLoadUI saveLoadUI;

        UICursorController cursorController;
        
        /// <summary>
        /// Initializes the scene UI installer with all required dependencies.
        /// </summary>
        public SceneUIInstaller(InventoryUI inventoryUI, BaseUI instructionsUI, SaveLoadUI saveLoadUI, UICursorController cursorController)
        {
            this.inventoryUI = inventoryUI;
            this.instructionsUI = instructionsUI;
            this.saveLoadUI = saveLoadUI;
            this.cursorController = cursorController;
        }
        /// <summary>
        /// Installs the application dependencies into each UI component.
        /// </summary>
        public void Initialize(GameBootstrapper bootstrapper, InventorySelectionController selectionController)
        {
            if (inventoryUI != null)
            {
                inventoryUI.Initialize(bootstrapper.InventorySystem, selectionController);
            }        

            if (saveLoadUI != null)
            {
                saveLoadUI.Initialize(bootstrapper.SaveGameService, bootstrapper.GameFlowService);
            }
        }
    }
}