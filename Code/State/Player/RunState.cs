using Code.Component.Items;
using Code.Component.Player;
using Code.Component.Player.Handlers;
using Code.State;
using UnityEngine;

namespace Code.Component.State
{
    public class RunState : IState
    {
        private CharacterControllerComponent controller;
        private IInput _input;
        private IAnimator _animator;
        private IMove _move;
        private IStatus _status;
        private IInteract _interact;
        private CharacterInventoryComponent _inventory;
        private InteractHandler _interactHandler;
        
        public RunState(CharacterControllerComponent controller, IInput input, IAnimator animator, IMove move, IStatus status, ISound _sound, IInteract interact, CharacterInventoryComponent inventory)
        {
            this.controller = controller;
            this._input = input;
            this._animator = animator;
            this._move = move;
            this._status = status;
            this._inventory = inventory;
            this._interactHandler = new InteractHandler(interact, inventory);
        }
        
        
        public void Enter()
        {
            _animator?.Play("Run");
        }

        public void Execute()
        {
            if (_input.MoveX != 0 || _input.MoveY != 0)
            {
                _move.DirectionMove(_input.MoveX, _input.MoveY);
            }
            if (_input.Interact.triggered)
            {
                _interactHandler.TryInteract();
            }
            if (_input.Jump.IsPressed())
            {
                controller.BaseStateMachine.ChangeState(controller.JumpState);
            }

            if ((int)_status.WeaponType == 1 && _input.Attack1.IsPressed())
            {
                controller.BaseStateMachine.ChangeState(controller.SwordAttack1State);
            }

            if ((int)_status.WeaponType == 2 && _input.Attack1.IsPressed())
            {
                controller.BaseStateMachine.ChangeState(controller.DaggerAttack1State);
            }

            if ((int)_status.WeaponType == 3 && _input.Attack1.IsPressed())
            {
                controller.BaseStateMachine.ChangeState(controller.BowAttack1State);
            }
            
            
            //No Input - Return to Idle
            if (_input.MoveX == 0 && _input.MoveY == 0)
            {
                controller.BaseStateMachine.ChangeState(controller.IdleState);
            }

            

        }

        public void Exit()
        {
            
        }
        
    }
}