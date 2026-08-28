namespace Game.Scenes
{
    public readonly struct SceneLoadResult
    {
        public string SceneName { get; }
        public float Duration { get; }

        public SceneLoadResult(string sceneName, float duration)
        {
            SceneName = sceneName;
            Duration = duration;
        }
    }
}