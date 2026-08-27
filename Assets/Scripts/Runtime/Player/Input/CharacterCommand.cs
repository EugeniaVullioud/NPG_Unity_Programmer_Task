using UnityEngine;

namespace Game.Character
{
    /// <summary>
    /// Represents the player's gameplay intent for the current simulation.
    /// This contains input intent, not movement or physics results.
    /// </summary>
    public struct CharacterCommand
    {
        public Vector2 Move;

        public bool JumpPressed;
        public bool PickupPressed;

        public CharacterCommand(Vector2 move, bool jumpPressed, bool pickupPressed)
        {
            Move = move;
            JumpPressed = jumpPressed;
            PickupPressed = pickupPressed;
        }
    }
}