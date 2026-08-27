using Game.Character;
using System;
using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// Defines the available strategies for detecting pickupable items.
    /// </summary>
    public enum ItemDetectionType
    {
        Sphere,
        ShpereRay,
    }
    /// <summary>
    /// Configuration asset responsible for creating an <see cref="IPickupDetector"/> using the selected item detection strategy.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDetectionStrategy", menuName = "Character/Pick Up/ Detection Strategy")]
    public class ItemDetectionStrategy : ScriptableObject
    {
        [SerializeField] PickupAbilitySettings _settings;
        [SerializeField] ItemDetectionType _type;

        public IPickupDetector Create() => Create(_type);

        public IPickupDetector Create(ItemDetectionType type)
        {
            return type switch
            {
                ItemDetectionType.Sphere => new SpherePickupDetector(_settings),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }
    }
}
