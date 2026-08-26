using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    /// <summary>
    /// Provides efficient runtime lookup of item definitions.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Inventory/Item Database")]
    public sealed class ItemDatabase : ScriptableObject
    {
        [SerializeField] List<ItemDefinition> definitions = new();

        Dictionary<string, ItemDefinition> lookup;

        /// <summary>
        /// Initializes the runtime lookup table.
        /// </summary>
        public void Initialize()
        {
            lookup = new Dictionary<string, ItemDefinition>(definitions.Count, StringComparer.Ordinal);

            for (int i = 0; i < definitions.Count; i++)
            {
                ItemDefinition definition = definitions[i];

                if (definition == null) continue;

                if (string.IsNullOrWhiteSpace(definition.Id))
                {
                    Debug.LogError($"Item '{definition.name}' has no ID.", definition);
                    continue;
                }

                if (!lookup.TryAdd(definition.Id, definition))
                {
                    Debug.LogError($"Duplicate item ID '{definition.Id}'.", definition);
                }
            }
        }

        /// <summary>
        /// Attempts to retrieve an item definition by ID.
        /// </summary>
        public bool TryGet(string id, out ItemDefinition definition)
        {
            if (lookup == null) Initialize();

            return lookup.TryGetValue(id, out definition);
        }
    }
}