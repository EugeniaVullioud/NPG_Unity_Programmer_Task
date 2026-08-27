using Game.Items;
using UnityEditor;
using UnityEngine;
namespace Game.Character
{
    /// <summary>
    /// Creates, registers, and manages the character's abilities and their initial unlock state.
    /// </summary>
    public sealed class CharacterAbilityInstaller : MonoBehaviour
    {
        CharacterAbilityController _abilityController;
        public CharacterAbilityController AbilityController => _abilityController;

        [SerializeField] CharacterMotor _characterMotor;

        [Header("Startup")]
        [SerializeField] CharacterAbilityStartupSettings _startupSettings;
        [SerializeField] ItemDetectionStrategy _detectionSettings;
        [SerializeField] ItemSelectorStrategy _selectionSettings;


        [Header("Ability Settings")]
        [SerializeField] PickupAbilitySettings _pickupSettings;
        [SerializeField] JumpAbilitySettings _jumpSettings;

        [SerializeField] Transform _pickupOrigin;

        PickupAbility _pickupAbility;
        JumpAbility _jumpAbility;

        void Start()
        {
            _abilityController = new CharacterAbilityController();

            InstallJumpAbility();
            InstallPickupAbility();
        }

        /// <summary>
        /// Creates and registers the character's jump ability using its configured settings.
        /// </summary>
        void InstallJumpAbility()
        {
            bool initiallyUnlocked = IsInitiallyUnlocked(AbilityId.Jump);
            _jumpAbility = new JumpAbility(_characterMotor.Rigidbody, _characterMotor.GroundProvider, _jumpSettings, initiallyUnlocked);
            _abilityController.Register(_jumpAbility);
        }
        // <summary>
        /// Creates and registers the character's pickup ability using its configured settings.
        /// </summary>
        void InstallPickupAbility()
        {
            bool initiallyUnlocked = IsInitiallyUnlocked(AbilityId.Pickup);

            var detector = _detectionSettings.Create();
            var selector = _selectionSettings.Create();
            _pickupAbility = new PickupAbility(transform, detector, selector, _pickupOrigin, _pickupSettings, initiallyUnlocked);
            _abilityController.Register(_pickupAbility);
        }
        /// <summary>
        /// Determines whether an ability should be unlocked when the character is initialized.
        /// </summary>
        /// <param name="abilityId">The identifier of the ability to check.</param>
        /// <returns>
        /// <see langword="true"/> if the ability is configured to start unlocked; otherwise, <see langword="false"/>.
        /// </returns>
        bool IsInitiallyUnlocked(AbilityId abilityId)
        {
            return _startupSettings != null && _startupSettings.IsInitiallyUnlocked(abilityId);
        }

        public void Unlock(AbilityId abilityId)
        {
            if (_abilityController.TryGetAbility<CharacterAbilityBase>(abilityId, out CharacterAbilityBase ability))
            {
                ability.Unlock();
            }
        }

        public void Lock(AbilityId abilityId)
        {
            if (_abilityController.TryGetAbility<CharacterAbilityBase>(abilityId, out CharacterAbilityBase ability))
            {
                ability.Lock();
            }
        }


#if UNITY_EDITOR

        void OnDrawGizmosSelected()
        {
            if (_pickupSettings == null || _pickupOrigin == null) return;

            Vector3 origin = _pickupOrigin.position;
            Vector3 forward = _pickupOrigin.forward;

            float range = _pickupSettings.Range;
            float maximumAngle = _pickupSettings.MaximumAngle;

            // Pickup range

            Handles.color = new Color(0f, 1f, 0f, 0.15f);

            Handles.DrawSolidDisc(origin, Vector3.up, range);

            Handles.color = Color.green;

            Handles.DrawWireDisc(origin, Vector3.up, range);

            // Forward direction

            Gizmos.color = Color.black;

            Gizmos.DrawLine(origin, origin + forward * range);        

            // Angle boundaries

            Vector3 leftDirection = Quaternion.AngleAxis(-maximumAngle, Vector3.up) * forward;
            Vector3 rightDirection = Quaternion.AngleAxis(maximumAngle, Vector3.up) * forward;

            Gizmos.color = Color.cyan;

            Gizmos.DrawLine(origin, origin + leftDirection * range);
            Gizmos.DrawLine(origin, origin + rightDirection * range);

            // Angle arc

            Handles.color = Color.cyan;

            Handles.DrawWireArc(origin, Vector3.up, leftDirection, maximumAngle * 2f, range);

            // Labels

            Handles.color = Color.white;

            Handles.Label(origin + forward * (range * 0.5f), $"Pickup Range: {range:0.00}");
            Handles.Label(origin + forward * range * 0.75f, $"Angle: {maximumAngle:0}°");
        }

#endif
    }
}