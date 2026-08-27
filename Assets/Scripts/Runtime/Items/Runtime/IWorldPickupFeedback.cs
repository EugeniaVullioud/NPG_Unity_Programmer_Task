namespace Game.Items
{
    /// <summary>
    /// Provides visual or other feedback for a world pickup object.
    /// </summary>
    public interface IWorldPickupFeedback
    {
        /// <summary>
        /// Sets whether the pickup should appear highlighted.
        /// </summary>
        void SetHighlighted(bool highlighted);

        /// <summary>
        /// Resets the pickup feedback to its default state.
        /// </summary>
        void ResetFeedback();
    }

}