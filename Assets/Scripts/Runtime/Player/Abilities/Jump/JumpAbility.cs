using UnityEngine;
namespace Game.Character
{
    /// <summary>
    /// Handles jump execution, jump buffering and coyote time.
    /// Ground detection remains outside this class. The jump ability consumes the ground state and decides whether jumping is allowed.
    /// </summary>
    public sealed class JumpAbility : CharacterAbilityBase, IFixedUpdateAbility
    {
        readonly Rigidbody _rigidbody;
        readonly IGroundStateProvider _groundProvider;
        readonly JumpAbilitySettings _settings;

        float _lastGroundedTime = float.NegativeInfinity;
        float _lastJumpPressedTime = float.NegativeInfinity;

        bool _wasGrounded;

        public override AbilityId Id => AbilityId.Jump;

        public JumpAbility(Rigidbody rigidbody, IGroundStateProvider groundProvider, JumpAbilitySettings settings, bool initiallyUnlocked) : base(initiallyUnlocked)
        {
            _rigidbody = rigidbody;
            _groundProvider = groundProvider;
            _settings = settings;
        }

        /// <summary>
        /// Receives a jump request. The jump may execute immediately or remain buffered until
        /// the character becomes eligible to jump.
        /// </summary>
        public override void Execute()
        {
            if (!IsUnlocked) return;
            _lastJumpPressedTime = Time.time;
        }

        public override bool CanExecute()
        {
            return IsUnlocked;
        }

        /// <summary>
        /// Updates grounded history and evaluates buffered jump requests.
        /// </summary>
        public void FixedUpdateAbility(float deltaTime)
        {
            if (!IsUnlocked) return;

            UpdateGroundedHistory();

            if (!HasBufferedJump()) return;
            if (!CanUseGroundJump()) return;

            PerformJump();
            ConsumeJumpRequest();
        }

        void UpdateGroundedHistory()
        {
            bool isGrounded = _groundProvider.CurrentState.IsGrounded;

            if (isGrounded) _lastGroundedTime = Time.time;

            _wasGrounded = isGrounded;
        }

        bool HasBufferedJump()
        {
            return Time.time <= _lastJumpPressedTime + _settings.JumpBufferTime;
        }

        bool CanUseGroundJump()
        {
            return Time.time <= _lastGroundedTime + _settings.CoyoteTime;
        }

        void PerformJump()
        {
            Vector3 velocity = _rigidbody.linearVelocity;

            velocity.y = _settings.JumpSpeed;

            _rigidbody.linearVelocity = velocity;
        }

        void ConsumeJumpRequest()
        {
            _lastJumpPressedTime = float.NegativeInfinity;
        }
    }
}