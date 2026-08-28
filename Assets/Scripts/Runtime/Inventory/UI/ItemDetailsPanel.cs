using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Inventory
{
    /// <summary>
    /// Displays detailed information about the currently selected
    /// or hovered item.
    /// </summary>
    public sealed class ItemDetailsPanel : MonoBehaviour
    {
        [SerializeField] TMP_Text nameText;
        [SerializeField] TMP_Text descriptionText;
        [SerializeField] TMP_Text rarityText;
        [SerializeField] TMP_Text quantityText;
        [SerializeField] TMP_Text modifiersText;
        [SerializeField] Image icon;

        void Awake()
        {
            Hide();
        }
        /// <summary>
        /// Shows information about an item.
        /// </summary>
        public void Show(ItemInstance item, ItemDefinition definition)
        {
            if (item == null || definition == null)
            {
                Hide();
                return;
            }

            gameObject.SetActive(true);

            if (nameText != null)
            {
                nameText.text = definition.DisplayName;
            }

            if (descriptionText != null)
            {
                descriptionText.text = definition.Description;
            }

            if (rarityText != null)
            {
                rarityText.text = definition.Rarity.ToString();
            }

            if (quantityText != null)
            {
                quantityText.text = $"Quantity: {item.Quantity}";
            }
    

            if (icon != null)
            {
                icon.sprite = definition.Icon;

                icon.enabled = definition.Icon != null;
            }

        }

        /// <summary>
        /// Hides the item details panel.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}