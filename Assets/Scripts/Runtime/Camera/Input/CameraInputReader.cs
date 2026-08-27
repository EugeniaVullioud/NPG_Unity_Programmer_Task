using UnityEngine;
using UnityEngine.InputSystem;
namespace Game.Camera
{
    /// <summary>
    /// Converts input actions into device-independent camera commands.
    /// Device-specific processing should happen before the command reaches the camera controller.
    /// </summary>
    public sealed class CameraInputReader : MonoBehaviour
    {
        [SerializeField] InputActionReference _lookAction;

        Vector2 _lookInput;

        void OnEnable()
        {
            if (_lookAction == null) return;

            _lookAction.action.Enable();

            _lookAction.action.performed += OnLookPerformed;
            _lookAction.action.canceled += OnLookCanceled;
        }

        void OnDisable()
        {
            if (_lookAction == null) return;

            _lookAction.action.performed -= OnLookPerformed;

            _lookAction.action.canceled -= OnLookCanceled;

            _lookAction.action.Disable();

            _lookInput = Vector2.zero;
        }

        /// <summary>
        /// Returns the current abstract camera command.
        /// </summary>
        public CameraCommand GetCommand()
        {
            return new CameraCommand(_lookInput);
        }

        void OnLookPerformed(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }

        void OnLookCanceled(InputAction.CallbackContext context)
        {
            _lookInput = Vector2.zero;
        }
    }
}