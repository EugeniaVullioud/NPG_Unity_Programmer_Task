using UnityEngine;

namespace Game.Character
{
    /// <summary>
    /// Resolves player movement input into a world-space direction relative to the camera.
    /// </summary>
    public static class CameraRelativeMovementResolver
    {
        const float DirectionEpsilon = 0.0001f;

        /// <summary>
        /// Converts 2D movement input into a normalized world-space direction relative to the camera.
        /// Falls back to the provided forward direction when the camera's horizontal forward is too small to produce a valid direction.
        /// </summary>
        /// <param name="input">The 2D movement input, where Y represents forward/backward and X represents left/right.</param>
        /// <param name="cameraTransform">The transform whose orientation determines the movement direction.</param>
        /// <param name="fallbackForward">The fallback forward direction used when the camera cannot provide a valid horizontal direction.</param>
        /// <returns>
        /// A world space movement direction with a maximum magnitude of 1, or <see cref="Vector3.zero"/> when there is no movement input.
        /// </returns>
        public static Vector3 Resolve(Vector2 input, Transform cameraTransform, Vector3 fallbackForward)
        {
            if (input.sqrMagnitude <= 0f) return Vector3.zero;

            Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up);

            if (forward.sqrMagnitude < DirectionEpsilon)
            {
                forward = Vector3.ProjectOnPlane(fallbackForward, Vector3.up);
            }

            forward.Normalize();

            Vector3 right = Vector3.Cross(Vector3.up, forward);
            Vector3 direction = forward * input.y + right * input.x;

            return Vector3.ClampMagnitude(direction, 1f);
        }
    }
}