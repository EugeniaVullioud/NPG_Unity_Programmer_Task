using System;

namespace Game.Inventory
{
    /// <summary>
    /// Persisted state of one item modifier.
    /// </summary>
    [Serializable]
    public sealed class ItemModifierSaveData
    {
        /// <summary>
        /// Unique modifier instance identifier.
        /// </summary>
        public string InstanceId;

        /// <summary>
        /// Modifier definition identifier.
        /// </summary>
        public string DefinitionId;

        /// <summary>
        /// Runtime modifier value.
        /// </summary>
        public float Value;
    }
}