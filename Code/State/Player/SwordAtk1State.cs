using UnityEngine;
using Code.State;

namespace Code.Component.State
{
    public class SwordAtk1State : IState
    {
        private CharacterControllerComponent controller;
        private IAnimator _animator;
        private IInteract _interact;
        private ISound _sound;
        
        private float enableAttackDelay = 0.3f;
        private float enableAttackTimer;
        
        private float attackDuration = 1.2f;
        private float attackTimer;
        
        private float _damageAmount;

        private float soundDelay = 0.3f;
        private float soundTimer;
        private bool atkSoundPlayed = false;
        
        public SwordAtk1State(CharacterControllerComponent controller, IInteract interact, IInput input, IAnimator animator, ISound _sound, float damageAmount)
        {
            this.controller = controller;
            this._animator = animator;
            this._interact = interact;
            this._sound = _sound;
            this._damageAmount = damageAmount;
        }

        public void Enter()
        {
            attackTimer = 0f;
            enableAttackTimer = 0f;
            _animator?.Play("SwordAtk1");
            atkSoundPlayed = false;
            soundTimer = 0f;
            

        }

        public void Execute()
        {
            // Debug.Log("Attack1 State");

            enableAttackTimer += Time.deltaTime;
            attackTimer += Time.deltaTime;
            soundTimer += Time.deltaTime;

            if (soundTimer >= soundDelay && !atkSoundPlayed)
            {
                atkSoundPlayed = true;
                _sound.PlaySound("SwordAttack1");
            }

            
            if (enableAttackTimer >= enableAttackDelay) _interact.EnableWeaponHit(_damageAmount);
            if (attackTimer >= attackDuration)
            {
                controller.BaseStateMachine.ChangeState(controller.IdleState);
            }
            
            

        }

        public void Exit()
        {
            _interact.DisableWeaponHit();
        }


    }
}

