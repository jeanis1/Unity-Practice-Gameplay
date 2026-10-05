using UnityEngine;
using Code.Component;
using Code.State;

namespace Code.Component.State
{
    public class DeathState : IState
    {
        private CharacterControllerComponent _controller;
        private IAnimator _animator;
        private IMove _move;
        private IStatus _status;
        private ISound _sound;
        private SkinnedMeshRenderer _characterMesh;
        private float _deathDelay = 5f;
        private float _deathTimer;

        public DeathState(CharacterControllerComponent controller, IAnimator animator, IMove move, IStatus status, ISound sound)
        {
            _controller = controller;
            _animator = animator;
            _move = move;
            _status = status;
            _sound = sound;
            _characterMesh = controller.GetComponentInChildren<SkinnedMeshRenderer>();
        }

        public void Enter()
        {
            _animator.Play("Dead");
            _sound.PlaySound("Death");
            _sound.PlaySound("OnHitSound");
            _deathTimer = 0f;
            
            if(_characterMesh == null) Debug.LogError("CharacterMesh is null");
        }

        public void Execute()
        {
            _deathTimer += Time.deltaTime;
            if (_deathTimer >= _deathDelay) _characterMesh.enabled = false; // hide mesh, but keep camera as player.
        }

        public void Exit()
        {
            
        }
    }
}
