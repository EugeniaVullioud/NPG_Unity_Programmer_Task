using System.Threading;
using System.Threading.Tasks;
namespace Game.Scenes
{
    public interface ISceneLoadingScreen
    {
        Task ShowAsync(CancellationToken cancellationToken);
        Task HideAsync(CancellationToken cancellationToken);
        void SetProgress(float progress);
    }
}