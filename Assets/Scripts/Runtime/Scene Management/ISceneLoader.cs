using System;
using System.Threading;
using System.Threading.Tasks;
namespace Game.Scenes
{
    public interface ISceneLoader
    {
        bool IsLoading { get; }

        Task<SceneLoadResult> LoadAsync(SceneLoadRequest request, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }
}