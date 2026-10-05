using UnityEngine;
using Code.Component.AI;
using Code.Component.Items.Pools;
using Code.State;

namespace Code.Component.State.AI
{
    public class AI_DeathState :  IState
    {
        private AI_ControllerComponent _controller;
        private IAI_Animator _animator;
        private IAIMove _move;
        private IAI_Status _status;
        private IAI_Sound _sound;
        private IPatrolling _patrolling;
        private GameObject _owningGameObject;
        private float _deathDelay = 3f;
        private float _deathTimer;
        private bool spawnedCoin = false;
        
        public AI_DeathState(AI_ControllerComponent controller, IAI_Status status, IAIMove move, IAI_Animator animator,
            IAI_Sound sound, GameObject gameObject, IPatrolling patrolling)
        {
            this._controller = controller;
            this._animator = animator;
            this._move = move;
            this._status = status;
            this._sound = sound;
            this._owningGameObject = gameObject;
            this._patrolling = patrolling;
        }
            
        public void Enter()
        {
            _animator.Play("Dead");
            _sound.PlaySound("Death");
            _sound.PlaySound("OnHitSound");
            _deathTimer = 0f;
            _patrolling.StopPatrol();
        }

        public void Execute()
        {
            // Debug.Log("AI Dead State");
            _deathTimer += Time.deltaTime;
            SpawnGold();
            if (_deathTimer >= _deathDelay) _owningGameObject.SetActive(false);
        }

        public void Exit()
        {
            spawnedCoin = false;
        }

        private void SpawnGold()
        {
            if(spawnedCoin) return;
            Vector3 CoinSpawn = new Vector3(0f, 1f, 0) + _owningGameObject.transform.position;
            CoinPool.Instance.SpawnCoin(CoinSpawn);
            spawnedCoin = true;
        }
        
        
    }
}

