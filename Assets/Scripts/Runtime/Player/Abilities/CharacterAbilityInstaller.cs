using UnityEngine;
namespace Game.Character
{
    /// <summary>
    /// Creates, registers, and manages the character's abilities and their initial unlock state.
    /// </summary>
    public sealed class CharacterAbilityInstaller : MonoBehaviour
    {
        CharacterAbilityController _abilityController;
        public CharacterAbilityController AbilityController =>_abilityController;

        [SerializeField] CharacterMotor _characterMotor;

        [Header("Startup")]
        [SerializeField] CharacterAbilityStartupSettings _startupSettings;


        [Header("Ability Settings")]
        [SerializeField] PickupAbilitySettings _pickupSettings;
        [SerializeField] JumpAbilitySettings _jumpSettings;

        [SerializeField] Transform _pickupOrigin;

        PickupAbility _pickupAbility;
        JumpAbility _jumpAbility;

        void Awake()
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
            _pickupAbility = new PickupAbility(transform, _pickupOrigin, _pickupSettings, initiallyUnlocked);
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
    }
}