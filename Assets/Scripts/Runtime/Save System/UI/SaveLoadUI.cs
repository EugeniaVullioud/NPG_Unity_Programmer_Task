using Game.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game.SaveSystem
{
    public enum SaveLoadMode
    {
        Save,
        Load
    }
    /// <summary>
    /// Displays and manages the save/load UI and its associated save slots.
    /// </summary>
    public sealed class SaveLoadUI : BaseUI
    {
        [SerializeField] SaveSlotUI slotPrefab;
        [SerializeField] Transform slotContainer;

        SaveGameService saveGameService;
        GameFlowService gameFlowService;

        SaveLoadMode mode;
        public SaveLoadMode Mode => mode;

        SaveSlotUI[] slots;
        IReadOnlyList<SaveSlotInfo> currentSlots;

        /// <summary>
        /// Initializes the save/load UI with the services required to perform
        /// save operations and control game flow.
        /// </summary>
        /// <param name="saveGameService">
        /// The service responsible for save game operations.
        /// </param>
        /// <param name="gameFlowService">
        /// The service responsible for controlling game flow.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when either service is <c>null</c>.
        /// </exception>
        public void Initialize(SaveGameService saveGameService, GameFlowService gameFlowService)
        {
            this.saveGameService = saveGameService ?? throw new ArgumentNullException(nameof(saveGameService));
            this.gameFlowService = gameFlowService ?? throw new ArgumentNullException(nameof(gameFlowService));

            Refresh();
        }
        public void SetMode(SaveLoadMode mode)
        {
            this.mode = mode;
            Refresh();
        }
        public void ShowSaveMode()
        {
            SetMode(SaveLoadMode.Save);
            Show();
        }
        public void ShowLoadMode()
        {
            SetMode(SaveLoadMode.Load);
            Show();
        }

        void Show()
        {
            Open();
            Refresh();
        }
        /// <summary>
        /// Refreshes the save slot UI using the latest save slot information.
        /// </summary>
        public void Refresh()
        {
            IReadOnlyList<SaveSlotInfo> saveSlots = saveGameService.GetSlots();
            currentSlots = saveSlots;

            EnsureSlotCount(saveSlots.Count);

            for (int i = 0; i < saveSlots.Count; i++)
            {
                slots[i].Bind(saveSlots[i], OnSlotSelected);
            }
        }

        void OnSlotSelected(int slotId)
        {
            if (!TryGetSlot(slotId, out SaveSlotInfo slot)) return;
            switch (mode)
            {
                case SaveLoadMode.Load:
                    HandleLoad(slot);
                    break;

                case SaveLoadMode.Save:
                    HandleSave(slot);
                    break;

                default:
                    Debug.LogError($"Unsupported save/load mode: {mode}", this);
                    break;
            }
        }
        void HandleLoad(SaveSlotInfo slot)
        {
            if (!slot.Exists) return;

            SaveOperationResult result = gameFlowService.LoadGame(slot.SlotId);

            if (!result.Success)
            {
                HandleFailure(result);
            }
        }

        void HandleSave(SaveSlotInfo slot)
        {
            if (slot.Exists)
            {
                // Confirmation should be presented here.
                // Once confirmed: SaveToSlot(slot.SlotId);
                return;
            }

            SaveToSlot(slot.SlotId);
        }

        void SaveToSlot(int slotId)
        {
            SaveOperationResult result = saveGameService.Save(slotId);

            if (!result.Success)
            {
                HandleFailure(result);
                return;
            }
            Refresh();
        }
        public void DeleteSlot(int slotId)
        {
            SaveOperationResult result = saveGameService.Delete(slotId);

            if (!result.Success)
            {
                HandleFailure(result);
                return;
            }
            Refresh();
        }

        bool TryGetSlot(int slotId, out SaveSlotInfo slot)
        {
            for (int i = 0; i < currentSlots.Count; i++)
            {
                if (currentSlots[i].SlotId == slotId)
                {
                    slot = currentSlots[i];
                    return true;
                }
            }

            slot = default;
            return false;
        }

        void HandleFailure(SaveOperationResult result)
        {
            Debug.LogWarning($"Save operation failed: {result.FailureReason}. {result.Message}", this);

            // This is the correct place to connect a future UI notification.
        }

        void EnsureSlotCount(int count)
        {
            if (slots != null && slots.Length == count) return;
            slots = new SaveSlotUI[count];

            for (int i = 0; i < count; i++)
            {
                slots[i] = Instantiate(slotPrefab, slotContainer);
            }
        }
    }
}