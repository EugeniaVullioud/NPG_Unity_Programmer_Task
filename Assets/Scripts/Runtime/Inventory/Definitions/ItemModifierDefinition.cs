using UnityEngine;

namespace Game.Inventory
{
    /// <summary>
    /// Defines the static behaviour of an item modifier. Modifier definitions are shared assets. 
    /// </summary>
    [CreateAssetMenu(fileName = "ItemModifierDefinition",menuName = "Game/Inventory/Item Modifier Definition")]
    public sealed class ItemModifierDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] string id;
        [SerializeField] string displayName;

        [Header("Statistics")]
        [SerializeField] ItemStat affectedStat;
        [SerializeField] ModifierOperation operation;

        public string Id => id;
        public string DisplayName => displayName;

        /// <summary>
        /// Gets the statistic affected by the modifier.
        /// </summary>
        public ItemStat AffectedStat => affectedStat;

        /// <summary>
        /// Gets the mathematical operation performed by the modifier.
        /// </summary>
        public ModifierOperation Operation => operation;
    }

    /// <summary>
    /// Defines how a modifier changes a statistic.
    /// </summary>
    public enum ModifierOperation
    {
        Add,
        Multiply
    }
}

