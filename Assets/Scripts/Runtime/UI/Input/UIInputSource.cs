using UnityEngine;
namespace Game.UI
{
    /// <summary>
    /// Registers this UI input source with a <see cref="UICursorController"/> while the component is enabled.
    /// Only add UIInputSource to UI that genuinely captures pointer interaction. Not constant gameplay UI.
    /// </summary>
    public sealed class UIInputSource : MonoBehaviour
    {
        [SerializeField] UICursorController cursorController;

        /// <summary>
        /// Registers this input source with the cursor controller when enabled.
        /// </summary>
        void OnEnable()
        {
            if (cursorController == null)
            {
                Debug.LogError("UIInputSource requires a UICursorController.", this);
                return;
            }

            cursorController.Register(this);
        }
        /// <summary>
        /// Unregisters this input source from the cursor controller when disabled.
        /// </summary>
        void OnDisable()
        {
            if (cursorController == null) return;

            cursorController.Unregister(this);
        }
    }
}