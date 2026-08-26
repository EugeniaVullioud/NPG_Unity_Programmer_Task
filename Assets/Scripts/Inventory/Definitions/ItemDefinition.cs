using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    /// <summary>
    /// Defines immutable data shared by all instances of an item. Contains configuration only.
    /// Runtime state such as durability, quantity and modifiers belongs to ItemInstance.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "Game/Inventory/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] string id;
        [SerializeField] string displayName;

        [TextArea]
        [SerializeField] string description;
        [SerializeField] Sprite icon;


        [Header("Stacking")]
        [Min(1)][SerializeField] int maxStackSize = 1;

        [Header("Can it be Equipped")]
        [SerializeField] bool equippable = false;

        [Header("Statistics")]
        [SerializeField] List<ItemStatValue> baseStats = new();

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;

        public Sprite Icon => icon;

        public bool IsEquippable => equippable;

        /// <summary>
        /// Gets the maximum number of items that can occupy one stack.
        /// </summary>
        public int MaxStackSize => maxStackSize;

        /// <summary>
        /// Gets the item's base statistics.
        /// </summary>
        public IReadOnlyList<ItemStatValue> BaseStats => baseStats;

        /// <summary>
        /// Attempts to retrieve a configured base statistic.
        /// </summary>
        public bool TryGetBaseStat(ItemStat stat, out float value)
        {
            for (int i = 0; i < baseStats.Count; i++)
            {
                if (baseStats[i].Stat == stat)
                {
                    value = baseStats[i].Value;
                    return true;
                }
            }
            value = 0f;
            return false;
        }
    }

}