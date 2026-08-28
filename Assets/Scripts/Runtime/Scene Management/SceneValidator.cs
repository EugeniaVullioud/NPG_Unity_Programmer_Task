using System.IO;
using UnityEngine.SceneManagement;
namespace Game.Scenes
{
    public sealed class SceneValidator : ISceneValidator
    {
        public bool IsValid(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName)) return false;

            int sceneCount = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < sceneCount; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

                if (string.IsNullOrEmpty(scenePath)) continue;

                string configuredSceneName = Path.GetFileNameWithoutExtension(scenePath);

                if (configuredSceneName == sceneName) return true;
            }

            return false;
        }
    }
}