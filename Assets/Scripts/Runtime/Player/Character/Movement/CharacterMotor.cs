using UnityEngine;

namespace Game.Character
{
    /// <summary>
    /// Controls character movement, rotation, and ground interaction using a Rigidbody-based motor.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class CharacterMotor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Rigidbody _rigidbody;

        [SerializeField] Transform _cameraTransform;

        [SerializeField] Transform _groundProbeOrigin;

        [Header("Settings")]
        [SerializeField] CharacterMovementSettings _movementSettings;

        [SerializeField] GroundProbeSettings _groundSettings;

        IGroundStateProvider _groundProvider;

        CharacterCommand _command;

        Vector3 _desiredWorldDirection;

        public GroundState GroundState => _groundProvider.CurrentState;
        public IGroundStateProvider GroundProvider => _groundProvider;

        public Vector3 Velocity => _rigidbody.linearVelocity;
        public Rigidbody Rigidbody => _rigidbody;

        void Reset()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        void Awake()
        {
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();

            _groundProvider = new SphereGroundProbe(_groundProbeOrigin, _groundSettings);

            ConfigureRigidbody();
        }
        public void EvaluateGround()
        {
            _groundProvider.Evaluate();
        }

        public void SimulateMovement(            float deltaTime)
        {

            ResolveMovementDirection();

            ApplyMovement(deltaTime);

            ApplyRotation(deltaTime);
        }

        /// <summary>
        /// Sets the movement command used by the character motor.
        /// </summary>
        /// <param name="command">The character command containing movement and action input.</param>
        public void SetCommand(in CharacterCommand command)
        {
            _command = command;
        }

        void ConfigureRigidbody()
        {
            _rigidbody.freezeRotation = true;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        /// <summary>
        /// Resolves the current movement input into a world-space movement direction.
        /// Uses the camera orientation when available and falls back to local character-relative movement otherwise.
        /// </summary>
        void ResolveMovementDirection()
        {
            if (_cameraTransform == null)
            {
                _desiredWorldDirection = new Vector3(_command.Move.x, 0f, _command.Move.y);
                _desiredWorldDirection = Vector3.ClampMagnitude(_desiredWorldDirection, 1f);
                return;
            }

            _desiredWorldDirection = CameraRelativeMovementResolver.Resolve(_command.Move, _cameraTransform, transform.forward);
        }
        /// <summary>
        /// Calculates and applies the character's horizontal movement while preserving vertical velocity.
        /// Movement acceleration and deceleration are controlled by the movement settings.
        /// </summary>
        void ApplyMovement(float deltaTime)
        {
            Vector3 currentVelocity = _rigidbody.linearVelocity;

            Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

            Vector3 targetDirection = ResolveSurfaceDirection(_desiredWorldDirection);

            float inputMagnitude = Mathf.Clamp01(_command.Move.magnitude);

            Vector3 targetVelocity = targetDirection * (_movementSettings.MaximumSpeed * inputMagnitude);

            float acceleration = inputMagnitude > 0f ? _movementSettings.Acceleration : _movementSettings.Deceleration;

            Vector3 newHorizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, acceleration * deltaTime);

            _rigidbody.linearVelocity = new Vector3(newHorizontalVelocity.x, currentVelocity.y, newHorizontalVelocity.z);
        }

        /// <summary>
        /// Projects the desired movement direction onto the current ground surface when grounded.
        /// </summary>
        /// <param name="desiredDirection">The desired world-space movement direction.</param>
        /// <returns>
        /// A movement direction aligned with the ground surface when grounded, or the original direction when not grounded.
        /// </returns>
        Vector3 ResolveSurfaceDirection(Vector3 desiredDirection)
        {
            if (!GroundState.IsGrounded) return desiredDirection;

            if (desiredDirection.sqrMagnitude <= 0f) return Vector3.zero;

            Vector3 projectedDirection = Vector3.ProjectOnPlane(desiredDirection, GroundState.Normal);

            if (projectedDirection.sqrMagnitude <= 0.0001f) return Vector3.zero;

            return projectedDirection.normalized;
        }
        /// <summary>
        /// Rotates the character toward the current desired movement direction.
        /// </summary>
        void ApplyRotation(float deltaTime)
        {
            if (_desiredWorldDirection.sqrMagnitude <= 0.0001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(_desiredWorldDirection, Vector3.up);

            Quaternion newRotation = Quaternion.RotateTowards(_rigidbody.rotation, targetRotation, _movementSettings.RotationSpeed * deltaTime);

            _rigidbody.MoveRotation(newRotation);
        }

#if UNITY_EDITOR

        void OnDrawGizmosSelected()
        {
            if (_groundProbeOrigin == null || _groundSettings == null) return;

            Vector3 origin = _groundProbeOrigin.position;

            float radius = _groundSettings.ProbeRadius;

            float distance = _groundSettings.NearGroundDistance;

            Gizmos.DrawWireSphere(origin, radius);

            Gizmos.DrawWireSphere(origin + Vector3.down * distance, radius);

            Gizmos.DrawLine(origin, origin + Vector3.down * distance);
        }

#endif
    }
}
