using UnityEngine;
namespace Game.Character
{
    public sealed class SphereGroundProbe : IGroundStateProvider
    {
        readonly Transform _probeOrigin;
        readonly GroundProbeSettings _settings;

        public GroundState CurrentState { get; private set; }

        public SphereGroundProbe(Transform probeOrigin, GroundProbeSettings settings)
        {
            _probeOrigin = probeOrigin;
            _settings = settings;

            CurrentState = GroundState.NotGrounded;
        }

        /// <summary>
        /// Evaluates the environment below the probe and updates the current ground state.
        /// </summary>
        public void Evaluate()
        {
            float castDistance = _settings.NearGroundDistance;

            Vector3 origin = _probeOrigin.position;

            bool hitSomething = Physics.SphereCast(origin, _settings.ProbeRadius, Vector3.down, out RaycastHit hit, castDistance, _settings.GroundMask, QueryTriggerInteraction.Ignore);

            if (!hitSomething)
            {
                CurrentState = GroundState.NotGrounded;
                return;
            }
            if (!IsWalkable(hit.normal))
            {
                CurrentState = GroundState.NotGrounded;
                return;
            }

            bool isGrounded = hit.distance <= _settings.SupportDistance;
            CurrentState = new GroundState(isGrounded, true, hit, hit.normal, hit.distance);
        }
        /// <summary>
        /// Determines whether a surface normal represents a walkable ground surface.
        /// </summary>
        bool IsWalkable(Vector3 normal) => Vector3.Dot(normal, Vector3.up) >= _settings.MinimumGroundDot;
    }
}