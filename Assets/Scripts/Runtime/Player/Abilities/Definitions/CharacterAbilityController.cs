using System.Collections.Generic;
namespace Game.Character
{
    /// <summary>
    /// Manages the character's available abilities and coordinates their execution.
    /// </summary>
    public sealed class CharacterAbilityController
    {
        readonly Dictionary<AbilityId, ICharacterAbility> _abilities = new();
        readonly List<IFixedUpdateAbility> _fixedUpdateAbilities = new();

        /// <summary>
        /// Registers an ability with the controller, replacing any existing ability with the same identifier.
        /// </summary>
        /// <param name="ability">The ability to register.</param>
        public void Register(ICharacterAbility ability)
        {
            if (ability == null)
            {
                UnityEngine.Debug.LogError("Cannot register a null ability.");
                return;
            }

            _abilities[ability.Id] = ability;

            if (ability is IFixedUpdateAbility fixedAbility)
            {
                _fixedUpdateAbilities.Add(fixedAbility);
            }
        }
        /// <summary>
        /// Attempts to execute the specified ability if it is registered, unlocked, and currently executable.
        /// </summary>
        /// <param name="abilityId">The identifier of the ability to execute.</param>
        /// <returns>
        /// <see langword="true"/> if the ability was found and executed successfully; otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryExecute(AbilityId abilityId)
        {
            if (!_abilities.TryGetValue(abilityId, out ICharacterAbility ability)) return false;
            if (!ability.IsUnlocked || !ability.CanExecute()) return false;

            ability.Execute();
            return true;
        }

        public bool TryGetAbility<T>(AbilityId abilityId, out T ability) where T : class, ICharacterAbility
        {
            if (_abilities.TryGetValue(abilityId, out ICharacterAbility registeredAbility))
            {
                ability = registeredAbility as T;
                return ability != null;
            }

            ability = null;
            return false;
        }

        public void Simulate(float deltaTime)
        {
            for (int i = 0; i < _fixedUpdateAbilities.Count; i++)
            {
                _fixedUpdateAbilities[i].FixedUpdateAbility(deltaTime);
            }
        }
    }
}