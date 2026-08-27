using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Character
{
    public class CharacterInputReader : MonoBehaviour
    {
        [SerializeField] InputActionReference _moveAction;
        [SerializeField] InputActionReference _jumpAction;
        [SerializeField] InputActionReference _pickupAction;

        Vector2 _moveInput;

        bool _jumpPressed;
        bool _pickupPressed;

        /// <summary>
        /// Enables the configured input actions and subscribes to their callbacks.
        /// </summary>
        void OnEnable()
        {
            _moveAction.action.Enable();
            _jumpAction.action.Enable();
            _pickupAction.action.Enable();

            _moveAction.action.performed += OnMovePerformed;
            _moveAction.action.canceled += OnMoveCanceled;

            _jumpAction.action.performed += OnJumpPerformed;
            _pickupAction.action.performed += OnPickupPerformed;
        }


        /// <summary>
        /// Unsubscribes from input callbacks and disables the configured input actions.
        /// </summary>
        void OnDisable()
        {
            _moveAction.action.performed -= OnMovePerformed;
            _moveAction.action.canceled -= OnMoveCanceled;

            _jumpAction.action.performed -= OnJumpPerformed;
            _pickupAction.action.performed -= OnPickupPerformed;

            _moveAction.action.Disable();
            _jumpAction.action.Disable();
            _pickupAction.action.Disable();

            _moveInput = Vector2.zero;
            _jumpPressed = false;
            _pickupPressed = false;
        }

        /// <summary>
        /// Returns the latest command and consumes one-frame action presses.
        /// Movement remains continuous while actions such as jump are edge-triggered.
        /// </summary>
        public CharacterCommand ConsumeCommand()
        {
            CharacterCommand command = new(_moveInput, _jumpPressed, _pickupPressed);

            _jumpPressed = false;
            _pickupPressed = false;

            return command;
        }

        /// <summary>
        /// Updates the movement input from the performed input action.
        /// </summary>
        void OnMovePerformed(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// Clears the movement input when the movement action is canceled.
        /// </summary>
        void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        /// <summary>
        /// Records that the jump action was pressed during the current frame.
        /// </summary>
        void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _jumpPressed = true;
        }

        void OnPickupPerformed(InputAction.CallbackContext context)
        {
            _pickupPressed = true;
        }
    }
}