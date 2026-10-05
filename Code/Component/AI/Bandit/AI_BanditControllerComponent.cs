using UnityEngine;
using Code.Component.State.AI;
using Code.State;
using UnityEngine.AI;


namespace Code.Component.AI.Bandit
{
    public class AIBanditControllerComponent : AI_ControllerComponent
    {

        
        public override void TakeDamage(Vector3 attackDirection, float damage)
        {
            if (!Status.Alive)
            {
                return;
            }
            Health.SubtractHealth(damage);

                if (Health.Health <= 0 && Status.Alive)
                {
                    Status.SetAlive(false);
                    StateMachine.ChangeState(DeathState);
                    Status.Death();
                    _gameManager.ReduceQuestCount();
                    Move.Knockback(attackDirection, 1f);
                    return;
                }
                Move.Knockback(attackDirection, 1f);
                Sound.PlaySound("OnHit");
                StateMachine.ChangeState(StunState);
            }
        }

}


