using Game.SaveSystem;
using System;
using UnityEngine;
namespace Game.UI
{
    public sealed class MainMenuUI : BaseUI
    {
        GameFlowService gameFlowService;

        public void Initialize(GameFlowService gameFlowService)
        {
            this.gameFlowService = gameFlowService ?? throw new ArgumentNullException(nameof(gameFlowService));
        }

        public void OnNewGameClicked()
        {
            gameFlowService.NewGame();
            Close();
        }
        public void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}