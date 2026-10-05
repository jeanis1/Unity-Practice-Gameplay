using Code.Component.AI;
using UnityEngine.AI;
using UnityEngine;
using Code.State;

namespace Code.Component.State.AI
{
    public class AI_PatrolState : IState
    {
        private AI_ControllerComponent _controller;
        private IAI_Animator _animator;
        private IPatrolling _patrolling;
        private NavMeshAgent _agent;
        private Transform _playerTransform;
        private bool _nextPatrolSet;
        private float _delayTimer;
        private float _delayDuration;
        private bool _chaseAttackEnabled;
        
        public AI_PatrolState(AI_ControllerComponent controller, IAI_Status status, IAIMove move, IAI_Animator animator, IAI_Sound sound, NavMeshAgent agent, Transform playerTransform, bool chaseAttackEnabled, IPatrolling patrolling)
        {
            this._controller = controller;
            this._animator = animator;
            this._agent = agent;
            this._playerTransform = playerTransform;
            this._chaseAttackEnabled = chaseAttackEnabled;
            this._patrolling = patrolling;
        }
        public void Enter()
        {
            // Debug.Log("Enter Patrol");
            _animator?.Play("Walk");
            _nextPatrolSet = false;
        }

        public void Execute()
        {
            // Debug.Log("Patrol State");
            _patrolling.PatrolMovement();

            float distToPlayer = Vector3.Distance(_controller.transform.position, _playerTransform.position);
            
            //chase
            if(distToPlayer <= _controller.chaseRange && _chaseAttackEnabled)
            {
                _controller.StateMachine.ChangeState(_controller.ChaseState);   
            }
            
            //Arrived patrol point
            if (!_agent.pathPending && _agent.remainingDistance < 1f && !_nextPatrolSet)
            {   
                _delayTimer = 0f;
                _nextPatrolSet = true;
                _delayDuration = Random.Range(2f, 5f);
                _animator?.Play("Idle");
            }
            
            //After delay, 
            if (_nextPatrolSet)
            {
                _delayTimer += Time.deltaTime;
                if (_delayTimer >= _delayDuration)
                {
                    _nextPatrolSet = false;
                    _patrolling.NextPatrolPoint();
                    _animator?.Play("Walk");
                }
            }
        }

        public void Exit()
        {
            
        }
    }
}
