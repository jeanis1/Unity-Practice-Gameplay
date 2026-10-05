using Code.Component.AI;
using Code.UI;
using UnityEngine;
using Code.Component;
using Code.Component.Player;

namespace Code.Game.GameState
{
    public class InGameState : IGameState
    {
        private readonly GameManager _gm;
        private readonly UIManager _ui;
        public GameStates States => GameStates.InGame;
        private GameObject[] _mapStartObjects;
        private float enemyCount;
        private float questEnemyCount;
        private CharacterControllerComponent _player;
        private AI_ControllerComponent[] _enemies;
        private GameObject[] _questEnemies;
        private CharacterInventoryComponent _playerInventory;

        public InGameState(GameManager gm, UIManager ui, GameObject[] mapStartObjects, int activeEnemyCount,GameObject[] questEnemies, CharacterControllerComponent player, AI_ControllerComponent[] enemies, CharacterInventoryComponent inventory)
        {
            _ui = ui;
            _mapStartObjects = mapStartObjects;
            _gm = gm;
            enemyCount = activeEnemyCount;
            _questEnemies = questEnemies;
            _player = player;
            _enemies = enemies;
            _playerInventory = inventory;
        }

        public void Enter()
        {
            _ui.Show(UIType.HUD); 
            //TODO - need to change code to respawn enemies (for new game loop) 
            for (int i = 0; i < _mapStartObjects.Length; ++i)
            {
                if (_mapStartObjects[i] == null) continue;
                _mapStartObjects[i].SetActive(true);

            }

            ResetGameState();

        }

        public void Exit() => _ui.Show(UIType.None); 

        public void Update()
        {
            //TODO - set  enemies not to destroy, but to hidden and update counter;
            if (enemyCount <= 0 && questEnemyCount <= 0)
            {
                _gm.NotifyPlayerWin();
            }
        }

        public void ResetGameState()
        {
            _gm.ResetItems();
            if(_player == null || _enemies == null) Debug.LogError("Enemy/PLayer reference not set GameManager.");
            _player.gameObject.SetActive(true);
            _player.ResetToInitialize();
            _playerInventory.EmptyInventory();
            
            foreach(var enemy in _enemies)
            {
                enemy.gameObject.SetActive(true);
                enemy.ResetToInitialize();
            }
        }

    }
}
