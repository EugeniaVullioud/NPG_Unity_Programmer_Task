using UnityEngine;

namespace Game.Camera
{
    /// <summary>
    /// Represents camera intent for the current frame.
    /// This contains abstract look intent and does not containevice-specific concepts such as mouse movement.
    /// </summary>
    public readonly struct CameraCommand
    {
        public Vector2 Look { get; }

        public CameraCommand(Vector2 look)
        {
            Look = look;
        }
    }
}