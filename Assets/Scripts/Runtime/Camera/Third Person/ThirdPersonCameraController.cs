using UnityEngine;
namespace Game.Camera
{
    /// <summary>
    /// Controls a third-person camera around a target.
    /// The controller is responsible for:
    /// - following a target
    /// - processing abstract look intent
    /// - calculating yaw and pitch
    /// - resolving camera obstruction
    /// </summary>
    public sealed class ThirdPersonCameraController : MonoBehaviour , ICameraOrientationProvider
    {
        [Header("References")]
        [SerializeField] Transform _target;

        [SerializeField] UnityEngine.Camera _camera;
        [SerializeField] ThirdPersonCameraSettings _settings;

        CameraCommand _command;

        float _yaw;
        float _pitch;

        Vector3 _pivotPosition;
        Vector3 _resolvedCameraPosition;

        public Transform Target => _target;

        public Vector3 PlanarForward
        {
            get
            {
                Vector3 forward = transform.forward;

                forward.y = 0f;

                float magnitude = forward.sqrMagnitude;

                if (magnitude <= 0.0001f) return Vector3.forward;
                return forward.normalized;
            }
        }

        public Vector3 PlanarRight
        {
            get
            {
                Vector3 right = transform.right;
                right.y = 0f;

                float magnitude = right.sqrMagnitude;

                if (magnitude <= 0.0001f) return Vector3.right;

                return right.normalized;
            }
        }

        void Awake()
        {
            Initialize();
        }

        void LateUpdate()
        {
            Simulate(Time.deltaTime);
        }

        /// <summary>
        /// Sets the target followed by this camera.
        /// </summary>
        public void SetTarget(Transform target)
        {
            _target = target;
            InitializeRotation();
        }

        /// <summary>
        /// Supplies abstract camera intent.
        /// </summary>
        public void SetCommand(in CameraCommand command)
        {
            _command = command;
        }

        /// <summary>
        /// Updates camera rotation and resolves the final camera position.
        /// </summary>
        public void Simulate(float deltaTime)
        {
            if (_target == null || _camera == null || _settings == null) return;

            UpdateRotation(_command, deltaTime);

            UpdatePivot();

            ResolveCameraPosition();

            ApplyCameraPose();
        }

        void Initialize()
        {
            if (_camera == null)
            {
                _camera = GetComponentInChildren<UnityEngine.Camera>();
                if (_camera == null) _camera = UnityEngine.Camera.main;
            }

            InitializeRotation();
        }

        void InitializeRotation()
        {
            Vector3 euler = transform.eulerAngles;

            _yaw = euler.y;

            _pitch = NormalizePitch(euler.x);
        }

        void UpdateRotation(in CameraCommand command, float deltaTime)
        {
            Vector2 look = command.Look;

            _yaw += look.x;

            _pitch -= look.y;
            _pitch = Mathf.Clamp(_pitch, _settings.MinimumPitch, _settings.MaximumPitch);

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        void UpdatePivot()
        {
            _pivotPosition = _target.position;
            transform.position = _pivotPosition;
        }

        void ResolveCameraPosition()
        {
            Vector3 direction = -transform.forward;

            float resolvedDistance = ResolveCameraDistance(direction);

            _resolvedCameraPosition = _pivotPosition + direction * resolvedDistance;

            //_camera.transform.LookAt(transform.position);
        }

        float ResolveCameraDistance(Vector3 direction)
        {
            float maxDistance = _settings.MaximumDistance;

            if (!Physics.SphereCast(_pivotPosition, _settings.CollisionRadius, direction, out RaycastHit hit, maxDistance, _settings.CollisionMask, QueryTriggerInteraction.Ignore))
            {
                return maxDistance;
            }

            float safeDistance = hit.distance - _settings.CollisionOffset;

            return Mathf.Clamp(safeDistance, _settings.MinimumDistance, maxDistance);
        }

        void ApplyCameraPose()
        {
            _camera.transform.SetPositionAndRotation(_resolvedCameraPosition, transform.rotation);
        }

        static float NormalizePitch(float angle)
        {
            if (angle > 180f) angle -= 360f;
            return angle;
        }
    }
}