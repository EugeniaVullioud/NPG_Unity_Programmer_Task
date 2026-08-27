using Game.Inventory;
using Game.Items;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Composes scene-local dependencies using application-level systems created by the GameBootstrapper.
/// </summary>
public sealed class SceneInstaller : MonoBehaviour
{
    [Header("Scene UI")]
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] GameBootstrapper bootstrapper;

    [Header("Scene Pickups")]
    [SerializeField] List<WorldItemPickup> pickups = new();

    ItemInstanceFactory itemFactory;

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

        Install(bootstrapper.InventorySystem);
        InitializeItems(bootstrapper.InventorySystem);
    }

    void Install(InventorySystem inventorySystem)
    {
        if (inventoryUI == null)
        {
            Debug.LogError("SceneInstaller requires an InventoryUI.", this);
            return;
        }

        inventoryUI.Initialize(inventorySystem);
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