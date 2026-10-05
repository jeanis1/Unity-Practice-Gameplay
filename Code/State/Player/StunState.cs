using System.Threading;
using Code.Component;
using UnityEngine;

namespace Code.State.Player
{
    public class StunState : IState
    {
        private CharacterControllerComponent _controller;
        private IAnimator _animator;
        private IMove _move;
        private IStatus _status;
        private float _stunDuration = 0.7f;
        private float _stunTimer;
        
        public StunState(CharacterControllerComponent controller, IAnimator animator, IMove move, IStatus status, ISound _sound)
        {
            this._controller = controller;
            this._animator = animator;
            this._move = move;
            this._status = status;
        }

        public void Enter()
        {
            _stunTimer = 0;
            _animator?.Play("Stun");
        }

        public void Execute()
        {
            _stunTimer += Time.deltaTime;
            if(_stunTimer >= _stunDuration) _controller.BaseStateMachine.ChangeState(_controller.IdleState);
        }

        public void Exit()
        {
            
        }
    }
}