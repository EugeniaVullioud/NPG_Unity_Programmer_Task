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
        public void Initialize(GameBootstrapper bootstrapper)
        {
            if (inventoryUI != null)
            {
                inventoryUI.Initialize(bootstrapper.InventorySystem);
            }        

            if (saveLoadUI != null)
            {
                saveLoadUI.Initialize(bootstrapper.SaveGameService, bootstrapper.GameFlowService);
            }
        }
    }
}
/*
[SerializeField] InventoryUI inventoryUI;
[SerializeField] SaveLoadUI saveLoadUI;
[SerializeField] InstructionsUI instructionsUI;
[SerializeField] PauseUI pauseUI;

[SerializeField] GameBootstrapper bootstrapper;
        [SerializeField] SceneUIConfiguration configuration;

void Start()
{
    InventorySystem inventory =
        bootstrapper.InventorySystem;

    inventoryUI?.Initialize(inventory);

    saveLoadUI?.Initialize(
        bootstrapper.SaveGameService,
        bootstrapper.GameFlowService);

    instructionsUI?.Initialize();

    pauseUI?.Initialize(); */