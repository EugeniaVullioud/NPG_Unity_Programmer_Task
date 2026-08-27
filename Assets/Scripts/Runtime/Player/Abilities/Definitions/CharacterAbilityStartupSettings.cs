using UnityEngine;

namespace Game.Character
{
    /// <summary>
    /// Defines which abilities should be available when the characteris initialized.
    /// This is useful for scene testing, debug scenes and different character presets without requiring progression to unlock abilities.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterAbilityStartupSettings", menuName = "Character/Ability Startup Settings")]
    public sealed class CharacterAbilityStartupSettings : ScriptableObject
    {
        [SerializeField] AbilityId[] _initiallyUnlockedAbilities;

        public bool IsInitiallyUnlocked(AbilityId abilityId)
        {
            if (_initiallyUnlockedAbilities == null) return false;

            for (int i = 0; i < _initiallyUnlockedAbilities.Length; i++)
            {
                if (_initiallyUnlockedAbilities[i] == abilityId) return true;
            }

            return false;
        }
    }
}