using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Inventory
{
    /// <summary>
    /// Presentation component for one inventory slot.
    /// It does not own inventory state.
    /// </summary>
    public sealed class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDropHandler, IPointerEnterHandler
    {
        [SerializeField] Image icon;

        [SerializeField] TMPro.TMP_Text quantityText;

        int slotIndex;

        InventoryService service;
        ItemDatabase database;

        /// <summary>
        /// Configures the UI slot.
        /// </summary>
        public void Bind(int slotIndex, InventoryService service, ItemDatabase database)
        {
            this.slotIndex = slotIndex;
            this.service = service;
            this.database = database;

            Refresh();
        }

        /// <summary>
        /// Refreshes the visual state from runtime inventory state.
        /// </summary>
        public void Refresh()
        {
            InventorySlot slot = service.Inventory.TryGetSlot(slotIndex);

            if (slot == null || slot.IsEmpty)
            {
                icon.enabled = false;
                quantityText.text = string.Empty;
                return;
            }

            ItemInstance item = slot.Item;

            if (!database.TryGet(item.DefinitionId, out ItemDefinition definition))
            {
                icon.enabled = false;
                quantityText.text = string.Empty;
                return;
            }

            icon.enabled = true;
            icon.sprite = definition.Icon;

            quantityText.text = item.Quantity > 1 ? item.Quantity.ToString() : string.Empty;
        }

        /// <inheritdoc />
        public void OnPointerClick(PointerEventData eventData)
        {
            service.Actions.Use(slotIndex);
        }

        /// <inheritdoc />
        public void OnBeginDrag(PointerEventData eventData)
        {
            InventoryUI.DraggedSlot = slotIndex;
        }

        /// <inheritdoc />
        public void OnDrop(PointerEventData eventData)
        {
            int source = InventoryUI.DraggedSlot;

            if (source < 0 || source == slotIndex) return;

            InventorySlot destination = service.Inventory.TryGetSlot(slotIndex);

            if (destination.IsEmpty)
            {
                service.Mutation.Move(source, slotIndex);
            }
            else
            {
                service.Mutation.Swap(source, slotIndex);
            }

            InventoryUI.DraggedSlot = -1;
        }

        /// <inheritdoc />
        public void OnPointerEnter(PointerEventData eventData)
        {
            service.Inventory.TryGetSlot(slotIndex);
        }
    }
}