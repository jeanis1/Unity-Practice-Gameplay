using Code.Component.Player;
using Code.Component.State;
using UnityEngine;
using Code.State;
using Code.State.Player;
using Code.UI;
using UnityEngine.Serialization;
using Cursor = UnityEngine.Cursor;

namespace Code.Component
{

    public class CharacterControllerComponent : MonoBehaviour, IDamageable, IResetObject
    {
        public BaseStateMachine BaseStateMachine { get; private set; }
        public IState IdleState { get; private set; }
        public IState RunState { get; private set; }
        public IState JumpState { get; private set; }
        public IState SwordAttack1State { get; private set; }
        public IState DaggerAttack1State { get; private set; }
        public IState BowAttack1State { get; private set; }
        public IState InteractState { get; private set; }
        public IState StunState { get; private set; }
        public IState DeathState { get; private set; }
        
        private IStatus _status; //Handles All Character Status related (hp, mp, equipped, stunned etc...)
        private IInput _input;
        private IMove _move;
        private IInteract _interact;
        private IAnimator _animator;
        private ISound _sound;
        private IHealth _health;
        
        private CharacterInventoryComponent _inventory;
        private Vector3 _startingPosition;
        private CharacterController _characterController;
        
        
        [FormerlySerializedAs("_ui")] [SerializeField] private UIManager ui;
        [SerializeField] private GameObject arrowPrefab;
        private bool _quitMenuOn;

        [SerializeField] private float swordDamage;
        [SerializeField] private float daggerDamage;
        [SerializeField] private float arrowDamage;
        
        
        void Awake()
        {
            _startingPosition  = transform.position;
            
            _status = GetComponent<IStatus>();
            _input = GetComponent<IInput>();
            _move = GetComponent<IMove>();
            _interact = GetComponent<IInteract>();
            _animator = GetComponent<IAnimator>();
            _sound =  GetComponent<ISound>();
            _inventory = GetComponent<CharacterInventoryComponent>();
            _health = GetComponent<IHealth>();
            
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            BaseStateMachine = new BaseStateMachine();
            IdleState = new IdleState(this, _input, _animator, _move, _status, _sound, _interact, _inventory);
            RunState = new RunState(this, _input, _animator, _move, _status, _sound, _interact, _inventory);
            JumpState = new JumpState(this, _input, _animator, _move,_status, _sound);
            SwordAttack1State = new SwordAtk1State(this, _interact, _input, _animator, _sound, swordDamage);
            DaggerAttack1State = new DaggerAtk1State(this, _interact, _animator, _sound, daggerDamage);
            BowAttack1State = new BowAtk1State(this, _interact, _animator, _sound, arrowPrefab, arrowDamage);
            StunState = new StunState(this, _animator, _move, _status, _sound);
            DeathState = new DeathState (this, _animator, _move, _status, _sound);

            if (_inventory == null)
            {
                Debug.LogError("CharacterInventoryComponent not found!", this);
            }
            _characterController = GetComponent<CharacterController>();
        }

        void Start()
        {
            BaseStateMachine.ChangeState(IdleState);
        }

        void Update()
        {
            BaseStateMachine.Update();

            EscapeCheck();
        }
        
        public void ResetToInitialize()
        {
            _characterController.enabled = false; //prevent issues with resetting player transform
            transform.position = _startingPosition; 
            _characterController.enabled = true;
            
            BaseStateMachine.ChangeState(IdleState);
            _status.SetAlive(true);
            _health.AddHealth(100);
            
            _interact.RemoveEquippedWeapon();
            _status.WeaponType = EquippedWeaponType.NoWeapon;

        }
        
        //if ESC Key pressed, Show/Hide Quit Menu
        private void EscapeCheck()
        {
            if (_input == null) return;
            if (_input.Escape.triggered && !_quitMenuOn)
            {
                ui.Show(UIType.QuitConfirm);
                _quitMenuOn = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                return;
            }
            if (_input.Escape.triggered && _quitMenuOn)
            {
                ui.GoBack();
                ui.Show(UIType.HUD);
                _quitMenuOn = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        
        public void TakeDamage(Vector3 attackDirection, float damage)
        {

            if(_health.Health <= 0 && !_status.Alive)
            {
                return; //can no longer be hit.
            }
            _health.SubtractHealth(damage);
            _sound.PlaySound("OnHit");                
            _move.Knockback(attackDirection, 1f);
            BaseStateMachine.ChangeState(StunState);
            if (_health.Health <= 0)
            {
                BaseStateMachine.ChangeState(DeathState);
            }
        }
        
        
    }
}