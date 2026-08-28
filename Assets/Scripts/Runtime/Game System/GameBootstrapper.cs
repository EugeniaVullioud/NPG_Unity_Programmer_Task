using Game.Inventory;
using Game.SaveSystem;
using Game.Scenes;
using System;
using UnityEngine;

public class GameBootstrapper: MonoBehaviour
{
    public static GameBootstrapper Instance { get; private set; }

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
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!ValidateConfiguration())
        {
            enabled = false;
            return;
        }


        itemDatabase.Initialize();

        SaveManager = new SaveManager();
        SaveGameService = new SaveGameService(SaveManager, saveSlotCount);


        ISceneValidator validator = new SceneValidator();
        SceneLoader = new SceneLoader(validator);

        InventorySystem = new InventorySystem(itemDatabase,  SaveManager, inventoryCapacity);

        InventorySystem.Initialize();
        GameFlowService = new GameFlowService(SaveGameService, SceneLoader, InventorySystem);

    }

    bool ValidateConfiguration()
    {
        if (itemDatabase == null)
        {
#if UNITY_EDITOR
            Debug.LogError("GameBootstrapper requires an ItemDatabase.", this);
#endif
            return false;
        }     
        return true;
    }
}
