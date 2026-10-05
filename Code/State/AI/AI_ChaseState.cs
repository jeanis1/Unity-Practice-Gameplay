using Code.Component.AI;
using Code.State;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Component.State.AI
{
    public class AI_ChaseState : IState
    {
        private AI_ControllerComponent _controller;
        private IAI_Animator _animator;
        private IAIMove _move;
        private IAI_Status _status;
        private IAI_Sound _sound;
        private IChasing _chasing;
        private NavMeshAgent _agent;
        private Transform _playerTransform;
        
   
        public AI_ChaseState(AI_ControllerComponent controller, IAI_Animator animator, Transform playerTransform, IChasing chasing)
        {
            this._controller = controller;
            this._animator = animator;
            this._playerTransform = playerTransform;
            this._chasing = chasing;
        }

        public void Enter()
        {
            _animator?.Play("Run");
        }

        public void Execute()
        {

            // Debug.Log("Chase State");
            _chasing.ChaseTarget();

            float distToPlayer = Vector3.Distance(_controller.transform.position, _playerTransform.position);
            // Debug.Log("Dist To Player: " + distToPlayer);
            
            if (distToPlayer > _controller.chaseRange)
            {
                //return to patrol
                _controller.StateMachine.ChangeState(_controller.PatrolState);
            }
            else if (distToPlayer <= _controller.attackRange)
            {
                //in bounds, Attack.
                // Debug.Log("Transition to AI Attack");
                _controller.StateMachine.ChangeState(_controller.AttackState);
            }
        }
        
        public void Exit()
        {
            
        }
    }
}