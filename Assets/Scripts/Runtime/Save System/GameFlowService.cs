using Game.Inventory;
using Game.Scenes;
using System.Linq;
using System.Threading.Tasks;

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

        InventorySystem inventory;
        public InventorySystem Inventory => inventory;
        /// <summary>
        /// Initializes a new instance of the <see cref="GameFlowService"/> class.
        /// </summary>
        public GameFlowService(SaveGameService saveGameService, ISceneLoader sceneLoadingService, InventorySystem inventory)
        {
            this.saveGameService = saveGameService;
            this.sceneLoadingService = sceneLoadingService;
            this.inventory = inventory;
        }

        /// <summary>
        /// Starts a new game by resetting or creating the required runtime state and transitioning to the appropriate game scene.
        /// </summary>
        public async Task<SceneLoadResult> NewGame()
        {
            // New games arent automatically saved in a slot.
            SceneLoadRequest sceneRequest = new SceneLoadRequest("SampleScene", true, true);
            return await sceneLoadingService.LoadAsync(sceneRequest);
        }

        /// <summary>
        /// Loads a saved game from the specified save slot and transitions to
        /// the scene associated with the save.
        /// </summary>
        public async Task<SaveOperationResult> LoadGame(int slotId)
        {
            if (slotId < 0)
            {
                return SaveOperationResult.Failed(SaveOperationFailureReason.SlotNotFound, $"Invalid save slot: {slotId}.");
            }

            if (!saveGameService.GetSlots().Any(slot => slot.SlotId == slotId && slot.Exists))
            {
                return SaveOperationResult.Failed(SaveOperationFailureReason.SlotNotFound, $"Save slot {slotId} does not exist.");
            }
            SceneLoadRequest sceneRequest = new SceneLoadRequest("SampleScene", true, true);

            await sceneLoadingService.LoadAsync(sceneRequest);

            // At this point SampleScene has been loaded.
            // SceneInstaller.Start() must have initialized InventorySystem before this call happens.
            SaveOperationResult result =         saveGameService.Load(slotId);

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