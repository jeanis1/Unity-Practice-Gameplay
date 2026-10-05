using System.Collections.Generic;
using Code.Component;
using Code.Component.AI;
using Code.Component.Player;
using Code.Game.GameState;
using UnityEngine;
using Code.UI;
using UnityEngine.Serialization;

namespace Code.Game
{
    public struct WeaponSpawnData
    {
        public GameObject Prefab; // asset references, surives destroy
        public Vector3 StartPosition;
        public Quaternion StartRotation;
    }
    
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public GameStateManager StateMachine { get; private set; }
        [SerializeField] private UIManager uiManager;
        [SerializeField] private GameObject[] mapStartObjects;
        private List<WeaponSpawnData> _startingItemsList = new List<WeaponSpawnData>(); //backup reference of starting items
        [FormerlySerializedAs("questEnemyObjects")] [SerializeField] private GameObject[] questEnemies;
        private CharacterControllerComponent _player;
        private AI_ControllerComponent[] _enemies;
        private CharacterInventoryComponent _playerInventory;
        [SerializeField] private bool resetOnZeroEnemies = true;

        //TODO - set based on spawn count later. (maybe separate gameObjects & enemyObjects into separate array later)
        [SerializeField] private int activeEnemyCount = 0; 
        
        [SerializeField] private int questEnemyCount;

    
    private void Awake()
        {
            //singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            
            StoreStartItemsInfo(mapStartObjects);
            
            _player = Object.FindObjectsByType<CharacterControllerComponent>(FindObjectsInactive.Include, FindObjectsSortMode.None)[0];
            _enemies = Object.FindObjectsByType<AI_ControllerComponent>(FindObjectsInactive.Include, FindObjectsSortMode.None); 
            _playerInventory = _player.gameObject.GetComponent<CharacterInventoryComponent>();
            
            //instantiate states
            var states = new IGameState[]
            { 
                new MainMenuState(uiManager, this),
                new SettingsState(uiManager),
                new InGameState(this, uiManager, mapStartObjects, activeEnemyCount, questEnemies, _player, _enemies, _playerInventory),
                new PlayerWinState(uiManager, this),
                new PlayerLoseState(uiManager, this), 
                
            };          
            
            StateMachine = new GameStateManager(states);

        }

        private void Start()
        {
            StateMachine.ChangeState(GameStates.MainMenu);

        }
        
        
        void Update()
        {
            StateMachine.Update();
        }
        
        //Bind UI Callback event handlers
        public void ToSettings()        => StateMachine.ChangeState(GameStates.Settings);
        public void ReturnToMainMenu()    => StateMachine.ChangeState(GameStates.MainMenu);
        public void StartGame()         => StateMachine.ChangeState(GameStates.InGame);
        public void NotifyPlayerWin ()  => StateMachine.ChangeState(GameStates.PlayerWin);
        public void NotifyPlayerLose () => StateMachine.ChangeState(GameStates.PlayerLose);
        
        public void ResetGame()
        {
            //implement Game Reset here...
            StateMachine.ChangeState(GameStates.InGame);
        }


        private void StoreStartItemsInfo(GameObject[] mapStartObjects)
        {
            for (int i = 0; i < mapStartObjects.Length; i++)
            {
                if (mapStartObjects[i] == null) continue;
                {
                    WeaponSpawnData data = new WeaponSpawnData
                    {
                        Prefab = mapStartObjects[i],
                        //References stored for game reset
                        StartPosition = mapStartObjects[i].transform.position,
                        StartRotation = mapStartObjects[i].transform.rotation
                        
                    };
                    _startingItemsList.Add(data);
                }
            }
        }

        public void ResetItems()
        {
            for (int i = 0; i < mapStartObjects.Length; i++)
            {
                if (mapStartObjects[i] != null) continue;
                {
                    CharacterInteractComponent interact = _startingItemsList[i].Prefab.GetComponent<CharacterInteractComponent>();
                    if (interact == null) continue;
                    interact.RemoveEquippedWeapon();
                    mapStartObjects[i].transform.position = _startingItemsList[i].StartPosition;
                    mapStartObjects[i].transform.rotation = _startingItemsList[i].StartRotation;
                    mapStartObjects[i].SetActive(true);
                    
                }
            }
        }
        
        public void ReduceEnemyCount()
        {
            activeEnemyCount--;
            if(activeEnemyCount <= 0 && resetOnZeroEnemies) StateMachine.ChangeState(GameStates.PlayerWin);
        }
        
        public void ReduceQuestCount()
        {
            questEnemyCount--;
            if(questEnemyCount == 0) Debug.Log("Quest Complete!");

        }

        public void ActivateQuest()
        {
            for (int i = 0; i < questEnemies.Length; i++)
            {
                questEnemies[i].SetActive(true);
                questEnemyCount++;
            }
        }
    }
}
