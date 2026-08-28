using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Inventory
{
    /// <summary>
    /// Handles input-driven actions for the inventory.
    /// </summary>
    public sealed class InventoryInputController : MonoBehaviour
    {
        [SerializeField] InputActionReference consumeAction;

        InventorySelectionController selection;
        InventoryService service;

        public void Bind(InventorySelectionController selection, InventoryService service)
        {
            this.selection = selection;
            this.service = service;
        }
        /// <summary>
        /// Consumes the currently selected inventory item.
        /// </summary>
        /// <remarks>
        /// If no inventory slot is currently selected, the action is ignored.
        /// </remarks>
        void OnConsume(InputAction.CallbackContext context)
        {
            int selectedSlot = selection.SelectedSlotIndex;

            if (selectedSlot < 0) return;

            var result = service.Actions.Use(selectedSlot);

#if UNITY_EDITOR
            // In the future , systems could be added to respond to consuming an item.
            if (!result.Success)
            {
                Debug.Log($"Unable to use selected item: {result.Failure}");
                return;
            }
#endif

        }
        /// <summary>
        /// Enables the  input actions and registers their callbacks.
        /// </summary>
        void OnEnable()
        {
            if (consumeAction != null)
            {
                consumeAction.action.performed += OnConsume;
                consumeAction.action.Enable();
            }

        }

        /// <summary>
        /// Disables the input actions and removes their callbacks.
        /// </summary>
        void OnDisable()
        {
            if (consumeAction != null)
            {
                consumeAction.action.performed -= OnConsume;
                consumeAction.action.Disable();
            }
        }
    }
}