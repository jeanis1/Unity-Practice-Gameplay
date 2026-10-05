using Code.Component;
using UnityEngine;
using Code.State;

namespace Code.State.Player
{
    public class DaggerAtk1State : IState
    {
        private CharacterControllerComponent controller;
        private IAnimator _animator;
        private IInteract _interact;
        private ISound _sound;
        private float _damageAmount;
        private float enableAttackDelay = 0.3f;
        private float enableAttackTimer;
        private float attackDuration = 1f;
        private float attackTimer;
        private float damageAmount = 5f;
        private float soundDelay = 0.1f;
        private float soundTimer;
        private bool atkSoundPlayed;
        
        public DaggerAtk1State(CharacterControllerComponent controller, IInteract interact,  IAnimator animator, ISound sound, float damageAmount)
        {
            this.controller = controller;
            this._animator = animator;
            this._interact = interact;
            this._sound = sound;
            this._damageAmount = damageAmount;
            
        }

        public void Enter()
        {
            attackTimer = 0f;
            enableAttackTimer = 0f;
            _animator?.Play("Stab");
            atkSoundPlayed = false;
            soundTimer = 0f;
        }

        public void Execute()
        {
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
