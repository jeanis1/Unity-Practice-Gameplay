using Code.Component.Items;
using Code.Component.Player;
using Code.Component.Player.Handlers;
using Code.State;
using UnityEngine;

namespace Code.Component.State
{
    public class IdleState : IState
    {
        private CharacterControllerComponent controller;
        private IInput _input;
        private IAnimator _animator;
        private IMove _move;
        private IStatus _status;
        private IInteract _interact;
        private GameObject interactTarget;
        private CharacterInventoryComponent _inventory;
        private InteractHandler _interactHandler;

        public IdleState(CharacterControllerComponent controller, IInput input, IAnimator animator, IMove move, IStatus status, ISound _sound, IInteract interact, CharacterInventoryComponent inventory)
        {
            this.controller = controller;
            this._input = input;
            this._animator = animator;
            this._move = move;
            this._status = status;
            this._interact = interact;
            this._inventory = inventory;
            this._interactHandler = new InteractHandler(interact, inventory);
        }


        public void Enter()
        {
            _animator?.Play("Idle");
            // if(_status == null) Debug.LogError("_Status not found ");

        }

        public void Execute()
        {
            // Debug.Log(_status.Equipped);
            // Debug.Log((int)_status.WeaponType);

            if (_input.Interact.triggered)
            {
                _interactHandler.TryInteract();
            }
            if (_input.MoveX != 0 || _input.MoveY != 0)
            {
                controller.BaseStateMachine.ChangeState(controller.RunState);
            }
            else if (_input.Jump.IsPressed() == true)
            {
                controller.BaseStateMachine.ChangeState(controller.JumpState);
            }
            else if ((int)_status.WeaponType == 1 && _input.Attack1.IsPressed() == true)
            {

                controller.BaseStateMachine.ChangeState(controller.SwordAttack1State);
            }
            else if ((int)_status.WeaponType == 2 && _input.Attack1.IsPressed() == true)
            {

                controller.BaseStateMachine.ChangeState(controller.DaggerAttack1State);
            }
            else if ((int)_status.WeaponType == 3 && _input.Attack1.IsPressed() == true)
            {
                controller.BaseStateMachine.ChangeState(controller.BowAttack1State);
            }

        }

        public void Exit()
        {

        }
        
    }
}