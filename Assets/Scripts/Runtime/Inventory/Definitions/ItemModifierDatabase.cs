using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    /// <summary>
    /// Provides efficient runtime lookup of modifier definitions.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemModifierDatabase", menuName = "Game/Inventory/Modifier Database")]
    public sealed class ItemModifierDatabase : ScriptableObject
    {
        [SerializeField] List<ItemModifierDefinition> definitions = new();

        Dictionary<string, ItemModifierDefinition> lookup;

        /// <summary>
        /// Initializes the runtime modifier lookup table.
        /// </summary>
        public void Initialize()
        {
            lookup = new Dictionary<string, ItemModifierDefinition>(definitions.Count, StringComparer.Ordinal);

            for (int i = 0; i < definitions.Count; i++)
            {
                ItemModifierDefinition definition = definitions[i];

                if (definition == null) continue;

                if (string.IsNullOrWhiteSpace(definition.Id))
                {
                    Debug.LogError($"Modifier '{definition.name}' has no ID.", definition);
                    continue;
                }
                if (!lookup.TryAdd(definition.Id, definition))
                {
                    Debug.LogError($"Duplicate modifier ID '{definition.Id}'.", definition);
                }
            }
        }

        /// <summary>
        /// Attempts to retrieve a modifier definition.
        /// </summary>
        public bool TryGet(string id, out ItemModifierDefinition definition)
        {
            if (lookup == null) Initialize();
            return lookup.TryGetValue(id, out definition);
        }
    }
}