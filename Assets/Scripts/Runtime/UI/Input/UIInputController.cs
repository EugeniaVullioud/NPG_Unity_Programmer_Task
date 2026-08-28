using UnityEngine;
using UnityEngine.InputSystem;
namespace Game.UI
{
    /// <summary>
    /// Handles input actions used to control the game's UI.
    /// </summary>
    public sealed class UIInputController : MonoBehaviour
    {
        [SerializeField] BaseUI inventoryUI;
        [SerializeField] BaseUI menuUI;

        [SerializeField] InputActionReference toggleInventoryAction;
        [SerializeField] InputActionReference toggleMenuAction;

        /// <summary>
        /// Enables the UI input actions and registers their callbacks.
        /// </summary>
        void OnEnable()
        {
            if (toggleInventoryAction != null)
            {
                toggleInventoryAction.action.performed += OnToggleInventory;
                toggleInventoryAction.action.Enable();
            }

            if (toggleMenuAction != null)
            {
                toggleMenuAction.action.performed += OnToggleMenu;
                toggleMenuAction.action.Enable();
            }
        }

        /// <summary>
        /// Disables the UI input actions and removes their callbacks.
        /// </summary>
        void OnDisable()
        {
            if (toggleInventoryAction != null)
            {
                toggleInventoryAction.action.performed -= OnToggleInventory;
                toggleInventoryAction.action.Disable();
            }

            if (toggleMenuAction != null)
            {
                toggleMenuAction.action.performed -= OnToggleMenu;
                toggleMenuAction.action.Disable();
            }
        }

        /// <summary>
        /// Toggles the inventory UI when the configured input action is performed.
        /// </summary>
        private void OnToggleInventory(InputAction.CallbackContext context)
        {
            if (inventoryUI == null)
            {
                Debug.LogError("UIInputController requires an InventoryUI.", this);
                return;
            }
            inventoryUI.Toggle();
        }

        /// <summary>
        /// Toggles the menu UI when the configured input action is performed.
        /// </summary>
        void OnToggleMenu(InputAction.CallbackContext context)
        {
            if (menuUI == null)
            {
                Debug.LogError("UIInputController requires a MenuUI.", this);
                return;
            }

            menuUI.Toggle();
        }
    }
}