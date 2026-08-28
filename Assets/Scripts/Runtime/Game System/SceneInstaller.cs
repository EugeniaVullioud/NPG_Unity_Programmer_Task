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

    ItemInstanceFactory itemFactory;

    SceneUIInstaller _UIInstaller; 
    void Start()
    {
        if (bootstrapper == null)
        {
            Debug.LogError("SceneInstaller requires a GameBootstrapper.", this);
            return;
        }

        if (bootstrapper.InventorySystem == null)
        {
            Debug.LogError("GameBootstrapper has not initialized the InventorySystem.", this);
            return;
        }

        Install();
        InitializeItems(bootstrapper.InventorySystem);
    }

    void Install()
    {
        _UIInstaller = new SceneUIInstaller(inventoryUI, instructionsUI, saveLoadUI, cursorController);
        _UIInstaller.Initialize(bootstrapper);
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
    }

}