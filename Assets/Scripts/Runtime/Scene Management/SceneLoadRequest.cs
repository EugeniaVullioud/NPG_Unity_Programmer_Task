using System;
namespace Game.Scenes
{
    public sealed class SceneLoadRequest
    {
        public string SceneName { get; }
        public bool ShowLoadingScreen { get; }
        public bool AllowSceneActivation { get; }

        public SceneLoadRequest(string sceneName, bool showLoadingScreen = true, bool allowSceneActivation = true)
        {
            if (string.IsNullOrWhiteSpace(sceneName)) throw new ArgumentException("Scene name is required.", nameof(sceneName));

            SceneName = sceneName;
            ShowLoadingScreen = showLoadingScreen;
            AllowSceneActivation = allowSceneActivation;
        }
    }
}