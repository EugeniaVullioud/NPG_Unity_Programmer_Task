using UnityEngine;
namespace Game.UI
{
    /// <summary>
    /// Provides common open, close, and toggle behavior for UI components.
    /// </summary>
    public abstract class BaseUI : MonoBehaviour
    {
        [SerializeField] protected GameObject _ui;
        public bool IsOpen => _ui.activeSelf;

        [SerializeField] protected bool _startOpen;
        /// <summary>
        /// Initializes the UI state when the component is created.
        /// </summary>
        protected void Awake()
        {
            if (_ui == null) _ui = gameObject;
            if (_startOpen) Open();
            else Close();
        }

        public void Open()
        {
            _ui.SetActive(true);
        }

        public void Close()
        {
            _ui.SetActive(false);
        }
        /// <summary>
        /// Toggles the UI between its open and closed states.
        /// </summary>
        public void Toggle()
        {
            if (IsOpen) Close();
            else Open();
        }
    }
}