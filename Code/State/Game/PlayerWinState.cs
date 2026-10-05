using Code.UI;

using UnityEngine;

namespace Code.Game.GameState
{
    public class PlayerWinState : IGameState
    {
        private readonly UIManager _ui;
        private readonly GameManager _gm;
        public GameStates States => GameStates.PlayerWin;
        private float resetDelay = 5f;
        private float resetTimer;
        public PlayerWinState(UIManager ui, GameManager gm)
        {
            _ui = ui;
            _gm = gm;
        }
        public void Enter()
        {
            resetTimer = 0f;
            _ui.Show(UIType.WinScreen);
        }

        public void Update()
        {
            resetTimer += Time.deltaTime;
            if (resetTimer >= resetDelay)
            {
                Debug.Log("Win Timer - Reset Timer");
                _gm.ResetGame();
            }
        }
        
        public void Exit()
        {
            _ui.Show(UIType.None);
        }
        
    }
}
