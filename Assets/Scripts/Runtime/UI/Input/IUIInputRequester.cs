namespace Game.UI
{
    /// <summary>
    /// Defines an interface for requesting pointer interaction from a UI system.
    /// </summary>
    public interface IUIInputRequester
    {
        /// <summary>
        /// Sets whether the specified requester requires pointer interaction.
        /// </summary>
        void SetPointerInteraction(object requester, bool requiresPointer);
    }

}