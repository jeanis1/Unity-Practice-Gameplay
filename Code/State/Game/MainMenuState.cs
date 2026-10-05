
using Code.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Game.GameState
{
    public class MainMenuState : IGameState
    {
        private readonly UIManager _ui;
        private readonly GameManager _gm;
        private Button playButton;
        private Button settingsButton;
        private Button quitButton;
        public GameStates States => GameStates.MainMenu;

        
        public MainMenuState(UIManager ui, GameManager gm)
        {
            _ui = ui;
            _gm = gm;
        }
        public void Enter()
        {
            _ui.Show(UIType.MainMenu);
        }
        public void Exit()
        {

        }
        public void Update()
        {
            
        }
    }
}
