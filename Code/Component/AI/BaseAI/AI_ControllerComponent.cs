using UnityEngine;
using Code.Component.State.AI;
using Code.Game;
using Code.State;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Code.Component.AI
{

    public interface IAI_Enemy
    {
        void PickupHealth(float healthAmount);
        void TakeDamage(Vector3 attackDirection, float damage);
        void BuffSpeed(float speedAmount);
        void RestoreOriginalSpeed();
    }
    public class AI_ControllerComponent : MonoBehaviour, IAI_Enemy, IResetObject
    {
        public BaseStateMachine StateMachine { get; private set; }
        public IState IdleState { get; private set; }
        public IState StunState { get; private set; }
        public IState ChaseState { get; private set; }
        public IState AttackState { get; private set; }
        public IState DeathState { get; private set; }
        public IState PatrolState { get; private set; }
        public IState FleeState { get; private set; }
        [SerializeField] protected GameManager _gameManager;

        
        [SerializeField] protected Transform playerTransform;
        private Vector3 startingAITransform;
        
        [FormerlySerializedAs("aggroRange")]
        public float chaseRange = 10f;
        public float attackRange = 2f;
        
        protected IAI_Status Status;
        protected IAIMove Move;
        protected IAI_Animator Animator;
        protected IAI_Sound Sound;
        protected NavMeshAgent Agent;
        protected IPatrolling Patrolling;
        protected IChasing Chasing;
        protected IFleeing Fleeing;
        protected ISpeedAdjustable SpeedAdjustable;
        protected IAI_Health Health;
        
        [SerializeField] protected bool _chaseAttackEnabled = true;
        
        [SerializeField] protected CapsuleCollider _attack1Collider;
        protected virtual void Awake()
        {
            startingAITransform = transform.position;
            
            Status = GetComponent<IAI_Status>();
            Animator = GetComponent<IAI_Animator>();
            Move = GetComponent<IAIMove>();
            Sound = GetComponent<IAI_Sound>();
            Agent = GetComponent<NavMeshAgent>();
            Patrolling = GetComponent<IPatrolling>();
            Chasing = GetComponent<IChasing>();
            Fleeing = GetComponent<IFleeing>();
            SpeedAdjustable = GetComponent<ISpeedAdjustable>();
            Health =  GetComponent<IAI_Health>();
            
            StateMachine = new BaseStateMachine();
            _attack1Collider.enabled = false;

            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }
            
            
            IdleState = new AI_IdleState(this, Status, Move, Animator, Sound, Agent, playerTransform, _chaseAttackEnabled);
            PatrolState = new AI_PatrolState(this, Status, Move, Animator, Sound, Agent, playerTransform, _chaseAttackEnabled, Patrolling);
            StunState = new AI_StunState(this, Status, Move, Animator, Sound);
            ChaseState = new AI_ChaseState(this, Status, Move, Animator, Sound, Agent, playerTransform, Chasing);
            AttackState = new AI_AttackState(this, Status, Move, Animator, Sound, playerTransform, _attack1Collider);
            DeathState = new AI_DeathState(this, Status, Move, Animator, Sound, gameObject, Patrolling);
            FleeState = new AI_FleeState(this, Status, Move, Animator, Sound, Agent, playerTransform, Fleeing);
        }
        
        public void ResetToInitialize()
        {
            StateMachine.ChangeState(IdleState);
            Status.SetAlive(true);
            Health.AddHealth(100); 
            transform.position = startingAITransform;
            Status.SetAlive(true);
            Debug.Log("AI Reset");
        }
        
        protected virtual void Start()
        {
            StateMachine.ChangeState(IdleState);
        }

        void Update()
        {
            StateMachine.Update();
            
            
        }


        public virtual void TakeDamage(Vector3 attackDirection, float damage)
        {
            if (!Status.Alive)
            {
                return;
            }
            
            Health.SubtractHealth(damage);
            
            if (Health.Health <= 0 && Status.Alive)
            {
                Status.SetAlive(false);
                Status.Death();
                StateMachine.ChangeState(DeathState);
                _gameManager.ReduceEnemyCount();
                Move.Knockback(attackDirection, 1f);
                return;
            }
            Move.Knockback(attackDirection, 1f);
            Sound.PlaySound("OnHit");
            StateMachine.ChangeState(StunState);
        }

        public void PickupHealth(float restoreAmount)
        {
            Health.AddHealth(restoreAmount);
            Sound.PlaySound("HealthRestore");
        }

        public void BuffSpeed(float speedAmount)
        {
            SpeedAdjustable.SetChaseSpeed(speedAmount + SpeedAdjustable.originalSpeed);
            Sound.PlaySound("SpeedBuff");
        }

        public void RestoreOriginalSpeed()
        {
            SpeedAdjustable.SetChaseSpeed(SpeedAdjustable.originalSpeed);
        }
    }
}








