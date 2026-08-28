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
        [SerializeField]
        private TMP_Text nameText;

        [SerializeField]
        private TMP_Text descriptionText;

        [SerializeField]
        private TMP_Text rarityText;

        [SerializeField]
        private TMP_Text quantityText;

        [SerializeField]
        private TMP_Text durabilityText;

        [SerializeField]
        private TMP_Text modifiersText;

        [SerializeField]
        private Image icon;

        /// <summary>
        /// Shows information about an item.
        /// </summary>
        public void Show(
            ItemInstance item,
            ItemDefinition definition)
        {
            if (item == null ||
                definition == null)
            {
                Hide();
                return;
            }

            gameObject.SetActive(true);

            if (nameText != null)
            {
                nameText.text =
                    definition.DisplayName;
            }

            if (descriptionText != null)
            {
                descriptionText.text =
                    definition.Description;
            }

            if (rarityText != null)
            {
                rarityText.text =
                    definition.Rarity.ToString();
            }

            if (quantityText != null)
            {
                quantityText.text =
                    $"Quantity: {item.Quantity}";
            }

            if (durabilityText != null)
            {
                if (definition.HasDurability)
                {
                    durabilityText.text =
                        $"Durability: " +
                        $"{item.Durability}/" +
                        $"{definition.MaximumDurability}";
                }
                else
                {
                    durabilityText.text =
                        string.Empty;
                }
            }

            if (icon != null)
            {
                icon.sprite =
                    definition.Icon;

                icon.enabled =
                    definition.Icon != null;
            }

            UpdateModifiers(
                item);
        }

        /// <summary>
        /// Hides the item details panel.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void UpdateModifiers(
            ItemInstance item)
        {
            if (modifiersText == null)
            {
                return;
            }

            if (item.Modifiers.Count == 0)
            {
                modifiersText.text =
                    string.Empty;

                return;
            }

            System.Text.StringBuilder builder =
                new System.Text.StringBuilder();

            for (int i = 0;
                 i < item.Modifiers.Count;
                 i++)
            {
                ItemModifierInstance modifier =
                    item.Modifiers[i];

                builder.Append(
                    modifier.DefinitionId);

                builder.Append(
                    ": ");

                builder.Append(
                    modifier.Value);

                if (i < item.Modifiers.Count - 1)
                {
                    builder.AppendLine();
                }
            }

            modifiersText.text =
                builder.ToString();
        }
    }
}