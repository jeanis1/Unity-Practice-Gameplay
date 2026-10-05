
using Code.Component.AI;
using Code.State;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Component.State.AI
{
    public class AI_IdleState : IState
    {
        private AI_ControllerComponent _controller;
        private IAI_Animator _animator;
        private IAIMove _move;
        private IAI_Status _status;
        private IAI_Sound _sound;
        private NavMeshAgent _agent;
        private float _idleTime = 2f;
        private float idleTimer;
        private Transform _playerTransform;
        private bool _chaseAttackEnabled;
        public AI_IdleState(AI_ControllerComponent controller, IAI_Status status, IAIMove move, IAI_Animator animator, IAI_Sound sound, NavMeshAgent agent, Transform playerTransform, bool chaseAttackEnabled)
        {
            this._controller = controller;
            this._animator = animator;
            this._move = move;
            this._status = status;
            this._sound = sound;
            this._agent = agent;
            this._playerTransform = playerTransform;
            this._chaseAttackEnabled = chaseAttackEnabled;
        }

        public void Enter()
        {
            _animator?.Play("Idle");
            idleTimer = 0f;
        }

        public void Execute()
        {
            idleTimer += Time.deltaTime;

            float distToPlayer = Vector3.Distance(_controller.gameObject.transform.position, _playerTransform.position);
            
            if (idleTimer >= _idleTime && distToPlayer <= _controller.chaseRange && _chaseAttackEnabled)
            {
                // Debug.Log("Transition To AI Chase");
                _controller.StateMachine.ChangeState(_controller.ChaseState);
            }
            else if (idleTimer >= _idleTime)
            {
                // Debug.Log("Transition To Patrol");
                _controller.StateMachine.ChangeState(_controller.PatrolState);
            }
        }

        public void Exit()
        {
            
        }


    }
}