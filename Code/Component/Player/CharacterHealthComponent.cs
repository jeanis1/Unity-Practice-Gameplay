using UnityEngine;
using System;

namespace Code.Component
{

    public interface IHealth
    {
        float Health { get; }
        float MaxHealth { get; }
        public void SubtractHealth(float amount);
        public void AddHealth(float amount);
    }

    public class CharacterHealthComponent : MonoBehaviour, IHealthReceiver, IHealth
    {
        [SerializeField] private float _health = 100f;
        [SerializeField] private float _maxHealth = 100f;

        public event Action<float, float> OnHealthChanged; // current, max
        public event Action OnDeath;

        public float Health => _health;
        public float MaxHealth => _maxHealth;

        void Awake()
        {
            OnHealthChanged?.Invoke(_health, _maxHealth);
        }

        public void AddHealth(float amount)
        {
            _health = Mathf.Min(_health + amount, _maxHealth);
            OnHealthChanged?.Invoke(_health, _maxHealth);
        }

        public void SubtractHealth(float amount)
        {
            _health -= amount;
            OnHealthChanged?.Invoke(_health, _maxHealth);

            if (_health <= 0)
            {
                _health = 0;
                OnDeath?.Invoke();
            }
        }
    }
}
