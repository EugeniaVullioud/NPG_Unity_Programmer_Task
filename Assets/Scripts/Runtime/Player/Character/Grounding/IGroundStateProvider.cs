namespace Game.Character
{
    public interface IGroundStateProvider
    {
        /// <summary>
        /// Gets the most recently evaluated ground state.
        /// </summary>
        GroundState CurrentState { get; }

        /// <summary>
        /// Evaluates the ground beneath the character and updates <see cref="CurrentState"/>.
        /// </summary>
        void Evaluate();
    }
}