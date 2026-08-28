using UnityEngine;

namespace Game.UI
{
    public sealed class PauseMenu : BaseUI
    {
        public void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}