using Code.Component.AI;
using Code.Component.AI.Bandit;
using UnityEngine.AI;
using UnityEngine;
using Code.State;

namespace Code.Component.State.AI.Bandit
{
    public class AI_BanditActivationState : IState
    {
        private AIBanditControllerComponent _controller;
        private IAI_Animator _animator;
        private NavMeshAgent _agent;
        private Transform _playerTransform;


        public AI_BanditActivationState(AIBanditControllerComponent controller, IAI_Animator animator, NavMeshAgent agent, Transform playerTransform)
        {
            this._controller = controller;
            this._animator = animator;
            this._agent = agent;
            this._playerTransform = playerTransform;
        }

        public void Enter()
        {
            _animator?.Play("Walk");
            
        }

        public void Execute()
        {
            
        }

        public void Exit()
        {
            
        }
    }
}

