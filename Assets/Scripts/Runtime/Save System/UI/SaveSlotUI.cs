using System;
using TMPro;
using UnityEngine;
namespace Game.SaveSystem
{
    /// <summary>
    /// Displays the state and metadata of an individual save slot.
    /// </summary>
    public sealed class SaveSlotUI : MonoBehaviour
    {
        [SerializeField] TMP_Text slotNameText;
        [SerializeField] TMP_Text sceneText;
        [SerializeField] TMP_Text dateText;
        [SerializeField] TMP_Text playtimeText;
        [SerializeField] GameObject emptyState;
        [SerializeField] GameObject occupiedState;

        int slotId;
        bool exists;

        public int SlotId => slotId;
        public bool Exists => exists;

        Action<int> onSelected;

        /// <summary>
        /// Binds the save slot UI to the specified save slot information.
        /// </summary>
        public void Bind(SaveSlotInfo info, Action<int> selectedCallback)
        {
            slotId = info.SlotId;
            exists = info.Exists;

            onSelected = selectedCallback;

            emptyState.SetActive(!info.Exists);
            occupiedState.SetActive(info.Exists);

            slotNameText.text = info.Exists ? info.DisplayName : "Empty Slot";
            sceneText.text = info.Exists ? info.SceneName : string.Empty;
            dateText.text = info.Exists ? info.LastModified.ToString("g") : string.Empty;
            playtimeText.text = info.Exists ? FormatPlaytime(info.Playtime) : string.Empty;
        }

        /// <summary>
        /// Notifies the registered callback that this save slot was selected.
        /// </summary>
        public void OnClicked()
        {
            onSelected?.Invoke(slotId);
        }

        public void OnDeleteClicked()
        {
            // Future implementation.
        }


        /// <summary>
        /// Formats a playtime duration as hours and minutes.
        /// </summary>
        /// <returns>
        /// The formatted playtime in <c>HH:MM</c> format.
        /// </returns>
        static string FormatPlaytime(TimeSpan playtime)
        {
            return $"{(int)playtime.TotalHours:00}:{playtime.Minutes:00}";
        }
    }
}
