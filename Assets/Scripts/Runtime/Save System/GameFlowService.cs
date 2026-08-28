using Game.Scenes;
using UnityEditor.Graphs;

namespace Game.SaveSystem
{
    /// <summary>
    /// Coordinates high-level game flow operations such as starting a new game,
    /// loading an existing game, and returning to the main menu.
    /// </summary>
    public sealed class GameFlowService
    {
        readonly SaveGameService saveGameService;
        readonly ISceneLoader sceneLoadingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameFlowService"/> class.
        /// </summary>
        public GameFlowService(SaveGameService saveGameService, ISceneLoader sceneLoadingService)
        {
            this.saveGameService = saveGameService;
            this.sceneLoadingService = sceneLoadingService;
        }

        /// <summary>
        /// Starts a new game by resetting or creating the required runtime state and transitioning to the appropriate game scene.
        /// </summary>
        public void NewGame()
        {
            // New games arent automatically saved in a slot.
            SceneLoadRequest sceneRequest = new SceneLoadRequest("SampleScene", true, true);
            sceneLoadingService.LoadAsync(sceneRequest);
        }

        /// <summary>
        /// Loads a saved game from the specified save slot and transitions to
        /// the scene associated with the save.
        /// </summary>
        public SaveOperationResult LoadGame(int slotId)
        {
            SaveOperationResult result = saveGameService.Load(slotId);

            if (!result.Success) return result;

            SceneLoadRequest sceneRequest = new SceneLoadRequest("SampleScene", true, true); // Remember to inject via save.

            var loadResult = sceneLoadingService.LoadAsync(sceneRequest);
            return result;
        }

        /// <summary>
        /// Returns the player to the main menu scene.
        /// </summary>
        public void ReturnToMainMenu()
        {
            SceneLoadRequest mainmenuRequest = new SceneLoadRequest("MainMenu", false, true);
            var loadResult = sceneLoadingService.LoadAsync(mainmenuRequest);
        }
    }
}