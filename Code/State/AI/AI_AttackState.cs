using Code.Component.AI;
using Code.State;
using UnityEngine;

namespace Code.Component.State.AI
{
    public class AI_AttackState : IState
    {
        private AI_ControllerComponent controller;
        private IAI_Animator _animator;
        private IAIMove _move;
        private IAI_Status _status;
        private IAI_Sound _sound;

        private Transform _playerTransform;
        private float rotateTimer = 0;
        private float rotateDuration = 2f;
        private float attackTimer = 0;
        private float attack1DelayTimer = 0.25f; //delay before activate hitbox
        private float _rotationSpeed = 360f;
        private bool _rotateComplete = false;
        private float _attackDuration = 1f;
        private bool attackOn = false;
        private bool isAttacking = false;
        private float RotationThreshold = 1f;
        private Collider _attack1Collider;

        private float _rotationTimeout = 0.5f; //Max time spent rotating
        private float _rotationTimer = 0f;
        private float _closeRangeDistance = 2f; //Distance to skip precise rotation
        
        
        
        public AI_AttackState(AI_ControllerComponent controller, IAI_Status status, IAIMove move, IAI_Animator animator, IAI_Sound sound, Transform playerTransform, CapsuleCollider attack1Collider)
        {
            this.controller = controller;
            this._animator = animator;
            this._move = move;
            this._status = status;
            this._sound = sound;
            this._playerTransform = playerTransform;
            this._attack1Collider = attack1Collider;
        }

        public void Enter()
        {
            _move.Stop();
            _animator?.Play("Attack1");
            _rotationTimer = 0f;
        }

        public void Execute()
        {
            if (!isAttacking)
            {
                RotateToPlayer();
            }
            else
            {
                HandleAttack();
            }
        }

        public void Exit()
        {
            ResetAttack();
            _move.Resume();
        }

        private void RotateToPlayer()
        {
            _rotationTimer += Time.deltaTime;
            
            //Check distance to player
            float distanceToPlayer = Vector3.Distance(controller.transform.position, _playerTransform.position);
            if (distanceToPlayer <= _closeRangeDistance || _rotationTimer >= _rotationTimeout)
            {
                StartAttack();
                return;
            }
            
            //Flatten direction Y axis to prevent pitch issue on slopes.
            Vector3 direction = (_playerTransform.position - controller.transform.position);
            direction.y = 0; //rotation horizontal only
            direction.Normalize();
            if (direction == Vector3.zero)
            {
                StartAttack();
                return;
            }
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            //calculate angle diff between current & target rotation
            float angleDifference = Quaternion.Angle(controller.transform.rotation, targetRotation);
            float dynamicThreshold = Mathf.Lerp(50f, 5f, angleDifference / 180f);
            
            controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            
            //attack, once in reasonable threshold
            if (angleDifference <= dynamicThreshold)
            {
                StartAttack();
            }
        }

        private void HandleAttack()
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attack1DelayTimer)
            {
                _attack1Collider.enabled = true;
            }
            
            if (attackTimer >= _attackDuration)
            {
                EndAttack();
            }
        }
        private void StartAttack()
        {
            _animator.Play("Attack1");
            isAttacking = true;
            attackTimer = 0f;
            _sound.PlaySound("Attack1");

        }

        private void EndAttack()
        {
            isAttacking = false;
            controller.StateMachine.ChangeState(controller.ChaseState);
            _attack1Collider.enabled = false;
        }
        private void ResetAttack()
        {
            isAttacking = false;
            attackTimer = 0f;
            _rotationTimer = 0f;
        }
    }
}

