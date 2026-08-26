using Game.Inventory;
using UnityEngine;

/// <summary>
/// Composes scene-local dependencies using application-level systems
/// created by the GameBootstrapper.
/// </summary>
public sealed class SceneInstaller : MonoBehaviour
{
    [Header("Scene UI")]
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] GameBootstrapper bootstrapper;

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

}