using Game.Scenes;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Game.Scenes
{
    public sealed class SceneLoader : ISceneLoader
    {
        readonly ISceneValidator _validator;
        readonly ISceneLoadingScreen _loadingScreen;

        public bool IsLoading { get; private set; }

        public SceneLoader(ISceneValidator validator, ISceneLoadingScreen loadingScreen = null)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _loadingScreen = loadingScreen;
        }

        public async Task<SceneLoadResult> LoadAsync(SceneLoadRequest request, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (IsLoading)
            {
                throw new InvalidOperationException("A scene is already being loaded.");
            }

            if (!_validator.IsValid(request.SceneName))
            {
                throw new InvalidOperationException($"Scene '{request.SceneName}' " + "is not included in Build Settings.");
            }

            IsLoading = true;

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                if (request.ShowLoadingScreen && _loadingScreen != null)
                {
                    await _loadingScreen.ShowAsync(cancellationToken);
                }

                AsyncOperation operation = SceneManager.LoadSceneAsync(request.SceneName);

                if (operation == null)
                {
                    throw new InvalidOperationException($"Failed to start loading scene " + $"'{request.SceneName}'.");
                }

                operation.allowSceneActivation = request.AllowSceneActivation;

                while (!operation.isDone)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    float normalizedProgress = Mathf.Clamp01(operation.progress / 0.9f);

                    progress?.Report(normalizedProgress);

                    _loadingScreen?.SetProgress(normalizedProgress);

                    // Unity's normal frame loop.
                    await Task.Yield();
                }

                progress?.Report(1f);

                _loadingScreen?.SetProgress(1f);

                if (request.ShowLoadingScreen && _loadingScreen != null)
                {
                    await _loadingScreen.HideAsync(cancellationToken);
                }

                stopwatch.Stop();

                return new SceneLoadResult(request.SceneName, (float)stopwatch.Elapsed.TotalSeconds);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}