using UnityEngine;
namespace Game.Camera
{
    /// <summary>
    /// Provides the camera's horizontal forward and right orientation vectors.
    /// </summary>
    public interface ICameraOrientationProvider
    {
        /// <summary>
        /// Gets the camera's forward direction projected onto the horizontal plane.
        /// </summary>
        Vector3 PlanarForward { get; }

        /// <summary>
        /// Gets the camera's right direction projected onto the horizontal plane.
        /// </summary>
        Vector3 PlanarRight { get; }
    }
}
