using Game.Inventory;
using Game.SaveSystem;
using System;
using UnityEngine;

public class GameBootstrapper: MonoBehaviour
{
    [Header("Definitions")]
    [SerializeField] ItemDatabase itemDatabase;
    [SerializeField] ItemModifierDatabase modifierDatabase;

    [Header("Inventory")]
    [Min(1)][SerializeField] int inventoryCapacity = 30;

    /// <summary>
    /// Gets the application's save manager.
    /// </summary>
    public SaveManager SaveManager { get; private set; }

    /// <summary>
    /// Gets the composed inventory system.
    /// </summary>
    public InventorySystem InventorySystem { get; private set; }

    void Awake()
    {
        if (!ValidateConfiguration())
        {
            enabled = false;
            return;
        }

        DontDestroyOnLoad(gameObject);

        itemDatabase.Initialize();
        modifierDatabase.Initialize();

        SaveManager = new SaveManager();

        InventorySystem = new InventorySystem(itemDatabase, modifierDatabase, SaveManager, inventoryCapacity);

        InventorySystem.Initialize();
    }

    bool ValidateConfiguration()
    {
        if (itemDatabase == null)
        {
            Debug.LogError("GameBootstrapper requires an ItemDatabase.", this);
            return false;
        }

        if (modifierDatabase == null)
        {
            Debug.LogError("GameBootstrapper requires an ItemModifierDatabase.", this);
            return false;
        }

        return true;
    }
}
