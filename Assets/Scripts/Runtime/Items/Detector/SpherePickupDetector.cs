using Game.Character;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Items
{
    /// <summary>
    /// Detects pickupable objects within a spherical area and filters them based on pickup range and the angle between the detection direction and the
    /// pickupable's position.
    /// </summary>
    public sealed class SpherePickupDetector : IPickupDetector
    {
        readonly PickupAbilitySettings _settings;
        readonly Collider[] _colliders;
        readonly HashSet<IPickupable> _uniquePickupables;

        readonly float _minimumDot;

        public SpherePickupDetector(PickupAbilitySettings settings)
        {
            _settings = settings;

            _colliders = new Collider[settings.MaxDetectionColliders];

            _uniquePickupables = new HashSet<IPickupable>();

            _minimumDot = Mathf.Cos(settings.MaximumAngle * Mathf.Deg2Rad);
        }
        /// <summary>
        /// Detects pickupable objects around the specified origin and filters them according to the configured range and viewing angle.
        /// </summary>
        /// <returns>
        /// The number of pickup candidates detected.
        /// </returns>
        public int Detect(Vector3 origin, Vector3 forward, List<PickupCandidate> results)
        {
            results.Clear();
            _uniquePickupables.Clear();

            int hitCount = Physics.OverlapSphereNonAlloc(origin, _settings.Range, _colliders, _settings.PickupMask, QueryTriggerInteraction.Collide);

            for (int i = 0; i < hitCount; i++)
            {
                Collider collider = _colliders[i];

                IPickupable pickupable = collider.GetComponentInParent<IPickupable>();

                if (pickupable == null || !collider.enabled) continue;

                if (!_uniquePickupables.Add(pickupable)) continue;

                Vector3 targetPosition = pickupable.PickupPosition;

                Vector3 offset = targetPosition - origin;

                float sqrDistance = offset.sqrMagnitude;

                if (sqrDistance > _settings.Range * _settings.Range) continue;

                float dot = Vector3.Dot(forward, offset.normalized);

                if (dot < _minimumDot) continue;

                float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                results.Add(new PickupCandidate(pickupable, collider, targetPosition, Mathf.Sqrt(sqrDistance), angle));
            }

            return results.Count;
        }
    }
}