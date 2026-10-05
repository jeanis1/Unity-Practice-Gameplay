using Code.UI;
using UnityEngine;

namespace Code.Game.GameState
{
    public class PlayerLoseState : IGameState
    {
        private readonly UIManager _ui;
        private readonly GameManager _gm;
        public GameStates States => GameStates.PlayerLose;
        private float resetDelay = 5f;
        private float resetTimer = 0f;

        public PlayerLoseState(UIManager ui, GameManager gm)
        {
            _ui = ui;
            _gm = gm;
        }
        public void Enter()
        {
            resetTimer = 0f;
            _ui.Show(UIType.LoseScreen);
        }
        public void Update()
        {
            resetTimer += Time.deltaTime;
            if (resetTimer >= resetDelay)
            {
                Debug.Log("Lose Timer -  Reset Game");
                _gm.ResetGame();
            }
        }

        public void Exit()
        {
            _ui.Show(UIType.None);
        }

        
    }
}
