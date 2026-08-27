namespace Game.Character
{
    /// <summary>
    /// Defines an ability that can be unlocked and executed by a character.
    /// </summary>
    public interface ICharacterAbility
    {
        /// <summary>
        /// Gets the unique identifier for this ability.
        /// </summary>
        AbilityId Id { get; }

        /// <summary>
        /// Gets whether this ability has been unlocked for use.
        /// </summary>
        bool IsUnlocked { get; }

        /// <summary>
        /// Determines whether the ability can currently be executed.
        /// </summary>
        /// <returns><see langword="true"/> when the ability can be executed; otherwise, <see langword="false"/>.</returns>
        bool CanExecute();

        /// <summary>
        /// Executes the ability.
        /// </summary>
        void Execute();
    }
}