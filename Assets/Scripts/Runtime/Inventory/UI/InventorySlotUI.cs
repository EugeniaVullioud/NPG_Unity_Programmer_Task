using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Inventory
{
    /// <summary>
    /// Presentation component for one inventory slot.
    /// It does not own inventory state.
    /// </summary>
    public sealed class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDropHandler, IPointerEnterHandler, IDragHandler, IEndDragHandler, IPointerExitHandler
    {
        InventoryDragController dragController;
        InventorySelectionController selectionController;
        InventoryDetailsController detailsController;

        [Header("Information")]
        [SerializeField] Image icon;
        [SerializeField] TMPro.TMP_Text quantityText;

        [Header("Selection")]
        [SerializeField] GameObject selectedVisual;

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
        /// Configures the drag controller.
        /// </summary>
        public void Bind(InventoryDragController dragController)
        {
            this.dragController = dragController;
        }
        /// <summary>
        /// Configures the selection and details controller.
        /// </summary>
        public void Bind(InventorySelectionController selectionController, InventoryDetailsController detailsController)
        {
            this.selectionController = selectionController;
            this.detailsController = detailsController;
        }

        /// <summary>
        /// Refreshes the visual state from runtime inventory state.
        /// </summary>
        public void Refresh()
        {
            InventorySlot slot = service.Inventory.TryGetSlot(slotIndex);

            if (slot == null || slot.IsEmpty)
            {
                NoSlotData();
                return;
            }

            ItemInstance item = slot.Item;

            if (!database.TryGet(item.DefinitionId, out ItemDefinition definition))
            {
                NoSlotData();
                return;
            }

            icon.enabled = true;
            icon.sprite = definition.Icon;

            quantityText.text = item.Quantity > 1 ? item.Quantity.ToString() : string.Empty;
        }
        void NoSlotData()
        {
            icon.enabled = false;
            quantityText.text = string.Empty;
            SetSelected(false);
        }

        /// <summary>
        /// Updates only the presentation of the selected state.
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (selectedVisual != null)
            {
                selectedVisual.SetActive(selected);
            }
        }
        /// <inheritdoc />
        public void OnPointerClick(PointerEventData eventData)
        {
            if (dragController != null && dragController.IsDragging) return;
            selectionController.Toggle(slotIndex);
        }

        /// <inheritdoc />
        public void OnBeginDrag(PointerEventData eventData)
        {
            InventoryUI.DraggedSlot = slotIndex;
            dragController.Begin(icon, eventData);
        }
        /// <inheritdoc />
        public void OnDrag(PointerEventData eventData)
        {
            dragController.UpdateDrag(eventData);
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

        public void OnEndDrag(PointerEventData eventData)
        {
            dragController.End();
            InventoryUI.DraggedSlot = -1; // was commented out
        }

        /// <inheritdoc />
        public void OnPointerEnter(PointerEventData eventData)
        {
            service.Inventory.TryGetSlot(slotIndex);
            detailsController.SetHoveredSlot(slotIndex);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            detailsController.ClearHoveredSlot(slotIndex);
        }

    }
}