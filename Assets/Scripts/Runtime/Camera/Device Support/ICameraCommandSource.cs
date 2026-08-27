namespace Game.Camera
{
    /// <summary>
    /// Provides camera control commands from an input or other camera command source.
    /// </summary>
    public interface ICameraCommandSource
    {
        CameraCommand GetCameraCommand();
    }
}