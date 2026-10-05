using Code.Component.AI;
using Code.State;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Component.State.AI
{
    public class AI_FleeState : IState
    {
        private AI_ControllerComponent _controller;
        private IAI_Status _status;
        private IAI_Animator _animator;
        private IAIMove _move;
        private IAI_Sound _sound;
        private IFleeing _fleeing;
        private NavMeshAgent _agent;
        private Transform _playerTransform;

        private float _fleeTimer;
        private float _fleeDuration;
        private const float MIN_FLEE_TIME = 1f;
        private const float MAX_FLEE_TIME = 3f;
        private const float VELOCITY_THRESHOLD = 5f;


        public AI_FleeState(AI_ControllerComponent controller, IAI_Status status, IAIMove move, IAI_Animator animator, IAI_Sound sound, NavMeshAgent agent, Transform playerTransform, IFleeing fleeing)
        {
            this._controller = controller;
            this._animator = animator;
            this._move = move;
            this._status = status;
            this._sound = sound;
            this._agent = agent;
            this._playerTransform = playerTransform;
            this._fleeing = fleeing;
        }

        public void Enter()
        {
            _fleeDuration = Random.Range(MIN_FLEE_TIME,MAX_FLEE_TIME);
            _fleeTimer = 0f;
            
            
            //Initialize Flee Position tracking
            if (_move is IAIMoveComponent moveComponent)
            {
                moveComponent.ResetFleePosition();
            }
        }

        public void Execute()
        {
            _fleeing.FleeFromTarget();
            
            _fleeTimer += Time.deltaTime;
            if (_fleeTimer >= _fleeDuration)
            {
                _controller.StateMachine.ChangeState(_controller.ChaseState);
                return;
            }

            float currentVelocity = _agent.velocity.magnitude;
            if (currentVelocity < VELOCITY_THRESHOLD)
            {
                _animator?.Play("Idle");
            }
            else
            {
                _animator?.Play("Run");
            }
        }

        public void Exit()
        {
            _agent.ResetPath();
        }
    }
}
