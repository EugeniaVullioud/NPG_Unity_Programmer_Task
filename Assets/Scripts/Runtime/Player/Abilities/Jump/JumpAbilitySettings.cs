using UnityEngine;
namespace Game.Character
{
    /// <summary>
    /// Configures the movement and input forgiveness settings used by the character's jump ability.
    /// </summary>
    [CreateAssetMenu(fileName = "JumpAbilitySettings", menuName = "Character/Abilities/Jump Settings")]
    public sealed class JumpAbilitySettings : ScriptableObject
    {
        [Header("Jump")]
        [SerializeField, Min(0f)] float _jumpSpeed = 7f;

        [Header("Forgiveness")]
        [Tooltip("How long after leaving valid ground the character can still jump.")]
        [SerializeField, Min(0f)] float _coyoteTime = 0.15f;

        [Tooltip("How long before landing a jump press can be buffered.")]
        [SerializeField, Min(0f)] float _jumpBufferTime = 0.15f;

        public float JumpSpeed => _jumpSpeed;

        public float CoyoteTime => _coyoteTime;

        public float JumpBufferTime => _jumpBufferTime;
    }
}