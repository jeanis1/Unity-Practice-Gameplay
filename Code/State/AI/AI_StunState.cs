using Code.Component.AI;
using Code.State;
using UnityEngine;

namespace Code.Component.State.AI
{
    public class AI_StunState : IState
    {
        private AI_ControllerComponent _controller;
        private IAI_Animator _animator;
        private IAIMove _move;
        private IAI_Status _status;
        private IAI_Sound _sound;
        
        private float _stunDuration = 0.7f;
        private float _stunTimer;
        
        public AI_StunState(AI_ControllerComponent controller, IAI_Status status, IAIMove move, IAI_Animator animator, IAI_Sound sound)
        {
            this._controller = controller;
            this._animator = animator;
            this._move = move;
            this._status = status;
            this._sound = sound;
            
        }
        
        public void Enter()
        {
            _stunTimer = 0f;
            _animator?.Play("Stun");
        }

        public void Execute()
        {
            _stunTimer += Time.deltaTime;
            if(_stunTimer >= _stunDuration) _controller.StateMachine.ChangeState(_controller.FleeState);
        }

        public void Exit()
        {
            
        }
        
    }
}