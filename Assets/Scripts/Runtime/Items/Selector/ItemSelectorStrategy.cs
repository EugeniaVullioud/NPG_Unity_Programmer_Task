using System;
using UnityEngine;
namespace Game.Items
{
    /// <summary>
    /// Defines the available strategies for selecting an item from detected pickupable items.
    /// </summary>
    public enum ItemSelectionType
    {
        Closest,
        Biggest,
    }
    /// <summary>
    /// Configuration asset responsible for creating an <see cref="IPickupSelector"/> using the selected item selection strategy.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemSelectionStrategy", menuName = "Character/Pick Up/ Selection Strategy")]
    public class ItemSelectorStrategy : ScriptableObject
    {
        [SerializeField] ItemSelectionType _type;

        public IPickupSelector Create() => Create(_type);
        public IPickupSelector Create(ItemSelectionType type)
        {
            return type switch
            {
                ItemSelectionType.Closest => new ClosestPickupSelector(),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }
    }
}
