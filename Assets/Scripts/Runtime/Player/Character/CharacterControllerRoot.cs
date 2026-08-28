using UnityEngine;
namespace Game.Character
{
    /// <summary>
    /// Coordinates character input, abilities, ground evaluation, and movement simulation.
    /// </summary>
    public sealed class CharacterControllerRoot : MonoBehaviour
    {
        [SerializeField] CharacterInputReader _inputReader;
        [SerializeField] CharacterMotor _motor;
        [SerializeField] CharacterAbilityInstaller _abilityInstaller;

        CharacterAbilityController _abilities;

        void Start()
        {
            _abilities = _abilityInstaller.AbilityController;
            _abilityInstaller.InstallJumpAbility(_motor);
        }
        /// <summary>
        /// Processes character input and advances abilities and movement during the physics update.
        /// </summary>
        void FixedUpdate()
        {
            CharacterCommand command =  _inputReader.ConsumeCommand();
            _motor.SetCommand(command);

            _motor.EvaluateGround();

            ProcessAbilityCommands(command);
            _abilities.Simulate(Time.fixedDeltaTime);

            _motor.SimulateMovement(Time.fixedDeltaTime);
        }
        /// <summary>
        /// Executes abilities requested by the current character command.
        /// </summary>
        /// <param name="command">The current character command containing ability input.</param>
        void ProcessAbilityCommands(in CharacterCommand command)
        {
            if (command.JumpPressed)
            {
                _abilities.TryExecute(AbilityId.Jump);
            }

            if (command.PickupPressed)
            {
                _abilities.TryExecute(AbilityId.Pickup);
            }
        }
    }
}