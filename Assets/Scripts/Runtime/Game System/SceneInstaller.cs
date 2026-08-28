using Game.UI;
using Game.Inventory;
using Game.Items;
using System.Collections.Generic;
using UnityEngine;
using Game.SaveSystem;

/// <summary>
/// Composes scene-local dependencies using application-level systems created by the GameBootstrapper.
/// </summary>
public sealed class SceneInstaller : MonoBehaviour
{
    [Header("Scene UI")]
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] UICursorController cursorController;
    [SerializeField] BaseUI instructionsUI;
    [SerializeField] SaveLoadUI saveLoadUI;

    [SerializeField] GameBootstrapper bootstrapper;

    [Header("Scene Pickups")]
    [SerializeField] List<WorldItemPickup> pickups = new();

    [Header("Inventory")]
    [SerializeField] InventoryInputController inventoryInputController;
    ItemInstanceFactory itemFactory;

    SceneUIInstaller _UIInstaller;

    // Should not be registered permanently by GameBootstrapper because its pickup list belongs to a particular scene.
    WorldSaveParticipant worldSaveParticipant;

    public bool IsInitialized { get; private set; }

    void Start()
    {
        Initialize();
    }
    void Initialize()
    {
        if (IsInitialized)            return;
        if (GameBootstrapper.Instance == null)
        {
            Debug.LogError("SceneInstaller requires a GameBootstrapper.", this);
            return;
        }

        if (GameBootstrapper.Instance.InventorySystem == null)
        {
            Debug.LogError("GameBootstrapper has not initialized the InventorySystem.", this);
            return;
        }

        Install();
        InitializeItems(GameBootstrapper.Instance.InventorySystem);
        IsInitialized = true;
    }

    void Install()
    {
        _UIInstaller = new SceneUIInstaller(inventoryUI, instructionsUI, saveLoadUI, cursorController);

        var selectionController = new InventorySelectionController(GameBootstrapper.Instance.InventorySystem.Inventory);
        _UIInstaller.Initialize(GameBootstrapper.Instance, selectionController);

        inventoryInputController.Bind(selectionController, GameBootstrapper.Instance.InventorySystem.Service);
    }
    void InitializeItems(InventorySystem inventorySystem)
    {
        itemFactory = new ItemInstanceFactory();
        IPickupInventoryReceiver receiver = new InventoryServicePickupReceiver(inventorySystem.Service);

        var operation = new PickupItemOperation(receiver, itemFactory);
        foreach (WorldItemPickup pickup in pickups)
        {
            if (pickup == null) continue;

            pickup.Initialize(operation);
        }
        worldSaveParticipant = new WorldSaveParticipant(pickups);

        GameBootstrapper.Instance.SaveManager.Register(worldSaveParticipant);
    }

}