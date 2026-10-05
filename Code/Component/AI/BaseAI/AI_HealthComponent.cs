using UnityEngine;
using System;
using Code.Component;

namespace Code.Component.AI
{
    public interface IAI_Health
    {
        void AddHealth(float amount);
        void SubtractHealth(float amount);
        float Health { get; } 
    }
    public class AI_HealthComponent : MonoBehaviour, IAI_Health
    {
        [SerializeField] private float _health = 100f;
        [SerializeField] private float _maxHealth = 100f;

        public event Action<float, float> OnHealthChanged; // current, max

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
        }


    }
}

