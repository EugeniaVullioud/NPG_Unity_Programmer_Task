namespace Game.Character
{
    /// <summary>
    /// Base implementation for character abilities that have unlockable state. This class does not know about Unity components or input.
    /// </summary>
    public abstract class CharacterAbilityBase : ICharacterAbility
    {
        public abstract AbilityId Id { get; }

        public bool IsUnlocked { get; private set; }

        protected CharacterAbilityBase(bool initiallyUnlocked)
        {
            IsUnlocked = initiallyUnlocked;
        }
        /// <summary>
        /// Unlocks the pickup ability.
        /// </summary>
        public void Unlock()
        {
            IsUnlocked = true;
        }
        /// <summary>
        /// Locks the pickup ability and prevents it from being executed.
        /// </summary>
        public void Lock()
        {
            IsUnlocked = false;
        }
        /// <summary>
        /// Determines whether the pickup ability is currently available for execution.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> when the ability is unlocked and its cooldown has elapsed;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public abstract bool CanExecute();

        /// <summary>
        /// Attempts to find and pick up a valid pickupable object in front of the character.
        /// </summary>
        public abstract void Execute();
    }
}