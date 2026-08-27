using UnityEngine;
namespace Game.Camera
{
    /// <summary>
    /// Root coordinator for the camera system.
    /// Responsible for connecting input to the active camera controller. It does not implement camera behaviour itself.
    /// </summary>
    public class CameraControllerRoot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] CameraInputReader _inputReader;
        [SerializeField] ThirdPersonCameraController _cameraController;

        bool _initialized;

        void Awake()
        {
            Initialize();
        }

        void OnEnable()
        {
            Initialize();
        }

        void Update()
        {
            if (!_initialized) return;

            ReadAndApplyCommand();
        }

        void Initialize()
        {
            if (_initialized) return;

            if (_inputReader == null)
            {
                _inputReader = GetComponentInChildren<CameraInputReader>();
            }

            if (_cameraController == null)
            {
                _cameraController = GetComponentInChildren<ThirdPersonCameraController>();
            }

            _initialized = true;
        }

        /// <summary>
        /// Reads the current camera input and sends it to the active camera controller.
        /// </summary>
        void ReadAndApplyCommand()
        {
            if (_inputReader == null || _cameraController == null) return;

            CameraCommand command = _inputReader.GetCommand();

            _cameraController.SetCommand(command);
        }

        /// <summary>
        /// Supplies an abstract camera command directly.
        /// Useful for AI, cutscenes, replays, tests, or alternative input systems.
        /// </summary>
        public void SetCommand(in CameraCommand command)
        {
            if (_cameraController == null) return;

            _cameraController.SetCommand(command);
        }

        /// <summary>
        /// Returns the current abstract camera command from the input source.
        /// </summary>
        public CameraCommand GetCommand()
        {
            if (_inputReader == null) return default;

            return _inputReader.GetCommand();
        }

        /// <summary>
        /// Changes the active camera target.
        /// </summary>
        public void SetTarget(Transform target)
        {
            if (!_initialized) Initialize();

            if (_cameraController == null) return;

            _cameraController.SetTarget(target);
        }

        /// <summary>
        /// Returns the currently controlled camera target.
        /// </summary>
        public Transform Target
        {
            get
            {
                if (!_initialized) Initialize();

                return _cameraController != null ? _cameraController.Target : null;
            }
        }

        /// <summary>
        /// Returns the active camera controller.
        /// </summary>
        public ThirdPersonCameraController CameraController
        {
            get
            {
                if (!_initialized) Initialize();

                return _cameraController;
            }
        }
    }
}