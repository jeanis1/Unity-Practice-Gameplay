using Code.State;
using UnityEngine;

namespace Code.Component.State
{
    public class JumpState : IState
    {
        private CharacterControllerComponent controller;
        private IInput _input;
        private IAnimator _animator;
        private IMove _move;
        private IStatus _status;
        
        public JumpState(CharacterControllerComponent controller, IInput input, IAnimator animator, IMove move, IStatus status, ISound _sound)
        {
            this.controller = controller;
            this._input = input;
            this._animator = animator;
            this._move = move;
            this._status = status;
        }

        void LateUpdate()
        {
        }
        
        public void Enter()
        {
            _animator?.Play("Jump");
            _move.Jump();
        }

        public void Execute()
        {
            
            // Air Move
            if (_input.MoveX != 0 || _input.MoveY != 0)
            {
                _move.DirectionMove(_input.MoveX, _input.MoveY);
            }

            //TODO - NEED TO CHANGE TO AIR ATTACK LATER!
            // Air Attack
            if ((int)_status.WeaponType == 1 && _input.Attack1.IsPressed())
            {
                controller.BaseStateMachine.ChangeState(controller.SwordAttack1State);
            }

            if ((int)_status.WeaponType == 2 && _input.Attack1.IsPressed())
            {
                controller.BaseStateMachine.ChangeState(controller.DaggerAttack1State);
            }
            
            //TODO - may need to add Jump Bow Attack
            
            if (_move.IsGrounded()) controller.BaseStateMachine.ChangeState(controller.IdleState);
        }
        
        public void Exit()
        {
            
        }
    }
}