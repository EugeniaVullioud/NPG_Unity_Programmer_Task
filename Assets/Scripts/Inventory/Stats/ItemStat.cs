using System;
using UnityEngine;

namespace Game.Inventory
{
    /// <summary>
    /// Identifies a statistic that can be associated with an item.
    /// Extend this enum when the game introduces additional item statistics.
    /// </summary>
    public enum ItemStat
        Health,
        Value
    }

    /// <summary>
    /// Represents a base statistic and its value.
    /// </summary>
    [Serializable]
    public struct ItemStatValue
    {
        [SerializeField] ItemStat stat;
        [SerializeField] float value;

        public ItemStat Stat => stat;
        public float Value => value;
    }
}