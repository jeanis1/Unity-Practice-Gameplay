using Code.Game;
using UnityEngine;

namespace Code.Component.AI
{
    // communicate to other components & actors character current Action
    public enum AI_ActionStatus
    {
        Idle,
        Wander,
        Stunned,
        Chase,
        Attack
    }
    public interface IAI_Status
    {
        AI_ActionStatus Action { get; }
        bool Alive { get; }
        void Death();
        void SetAlive(bool alive);
    }

    public class AI_StatusComponent : MonoBehaviour, IAI_Status
    {
        
        private AI_HealthComponent _healthComponent;

        public float Health => _healthComponent != null ? _healthComponent.Health : 0f; // read only

        //TODO - update ActionState during each state change
        public AI_ActionStatus Action { get; } = AI_ActionStatus.Idle;

        public bool Alive { get;  private set; } = true;
    
    

        void Awake()
        {
            
        }

        
        public void Death()
        {
            if (!Alive) return;//prevent inf recursion
            Alive = false;
        }

        public void SetAlive(bool alive)
        {
            Alive = alive;
        }
    }
    
}

