using Code.Game;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Analytics;

namespace Code.Component
{
    // communicate to other components & actors character current Action
    public enum CharacterActionStatus
    {
        NoAction,
        Stunned,
        Attack1
    }

    public enum EquippedWeaponType
    {
        NoWeapon,
        BaseSword,
        BaseDagger,
        BaseBow
    }

    public interface IStatus
    {
        float Health { get; }
        void AddHealth(float healthAmount);
        void SubtractHealth(float healthAmount);
        CharacterActionStatus Action { get; set; }
        bool Alive { get; }
        void Death();
        void SetAlive(bool isAlive);
        EquippedWeaponType WeaponType { get; set; }

    }

    public class CharacterStatusComponent : MonoBehaviour, IStatus
    {
        [SerializeField] private float attackStrength = 100f; //100% regular strength TODO - need to apply this to item damage multipliers for real application
        [SerializeField] private GameManager _gameManager;

        private CharacterHealthComponent _healthComponent;
        private CharacterStateComponent _stateComponent;
        public bool Alive { get; private set; } = true;
        
        public float Health => _healthComponent != null ? _healthComponent.Health : 0f; //Read Only

        //TODO - update ActionState during each state change (prevents overwriting current action with another)
        public CharacterActionStatus Action
        {
            get => _stateComponent != null ? _stateComponent.CurrentAction : CharacterActionStatus.NoAction;
            set { if (_stateComponent != null) _stateComponent.CurrentAction = value; }
        }

        public EquippedWeaponType WeaponType
        {
            get => _stateComponent != null ? _stateComponent.WeaponType : EquippedWeaponType.NoWeapon;
            set { if (_stateComponent != null) _stateComponent.WeaponType = value; }
        }


        void Awake()
        {
            _healthComponent = GetComponent<CharacterHealthComponent>();
            _stateComponent = GetComponent<CharacterStateComponent>();

            if (_healthComponent == null)
            {
                Debug.LogError("CharacterHealthComponent not found! Please add it to the GameObject.", this);
            }

            if (_stateComponent == null)
            {
                Debug.LogError("CharacterStateComponent not found! Please add it to the GameObject.", this);
            }

            if (_healthComponent != null)
            {
                _healthComponent.OnDeath += Death;
            }
        }

        void OnDestroy()
        {
            if (_healthComponent != null)
            {
                _healthComponent.OnDeath -= Death;
            }
        }

        public void AddHealth(float healthAmount)
        {
            _healthComponent?.AddHealth(healthAmount);
        }

        public void SubtractHealth(float healthAmount)
        {
            _healthComponent?.SubtractHealth(healthAmount);
        }

        public void Death()
        {
            _gameManager.NotifyPlayerLose();
        }

        public void AddStrength(float strength)
        {
            attackStrength += strength;
        }

        public void SubtractStrength(float strength)
        {
            attackStrength -= strength;
        }

        public void SetAlive(bool isAlive)
        {
            Alive = isAlive;
        }
    }
}
