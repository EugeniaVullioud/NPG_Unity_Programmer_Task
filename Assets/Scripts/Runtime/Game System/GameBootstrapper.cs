using Game.Inventory;
using Game.SaveSystem;
using Game.Scenes;
using System;
using UnityEngine;

public class GameBootstrapper: MonoBehaviour
{
    [Header("Definitions")]
    [SerializeField] ItemDatabase itemDatabase;

    [Header("Inventory")]
    [Min(1)][SerializeField] int inventoryCapacity = 30;


    [Header("Save System")]
    [Min(1)] [SerializeField] int saveSlotCount = 5;
    /// <summary>
    /// Gets the application's save manager.
    /// </summary>
    public SaveManager SaveManager { get; private set; }

    /// <summary>
    /// Gets the composed inventory system.
    /// </summary>
    public InventorySystem InventorySystem { get; private set; }

    public ISceneLoader SceneLoader { get; private set; }

    public GameFlowService GameFlowService { get; private set; }

    public SaveGameService SaveGameService { get; private set; }
    void Awake()
    {
        if (!ValidateConfiguration())
        {
            enabled = false;
            return;
        }

        DontDestroyOnLoad(gameObject);

        itemDatabase.Initialize();

        SaveManager = new SaveManager();
        SaveGameService = new SaveGameService(SaveManager, saveSlotCount);

        InventorySystem = new InventorySystem(itemDatabase,  SaveManager, inventoryCapacity);

        InventorySystem.Initialize();

        ISceneValidator validator = new SceneValidator();
        SceneLoader = new SceneLoader(validator);

        GameFlowService = new GameFlowService(SaveGameService, SceneLoader);
    }

    bool ValidateConfiguration()
    {
        if (itemDatabase == null)
        {
            Debug.LogError("GameBootstrapper requires an ItemDatabase.", this);
            return false;
        }     
        return true;
    }
}
