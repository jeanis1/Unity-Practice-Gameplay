using UnityEngine;
using System.Collections;
using UnityEngine.AI;


namespace Code.Component.AI
{
    public interface IAIMove
    {
        void Stop();
        void Resume();
        void Knockback(Vector3 attackDirection, float knockbackForce);
    }

    public interface ISpeedAdjustable
    {
        float originalSpeed { get; set; }
        void SetChaseSpeed(float speed);
    }

    public interface 
        IPatrolling
    {
        void PatrolMovement();
        void NextPatrolPoint();
        void StopPatrol();
    }

    public interface IChasing
    {
        void ChaseTarget();

    }

    public interface IFleeing
    {
        void FleeFromTarget();

    }

public class IaiMoveComponent : MonoBehaviour, IAIMove, ISpeedAdjustable,IPatrolling, IChasing, IFleeing
    {
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float chaseSpeed = 5f;
        [SerializeField] private float fleeSpeed = 8f;
        [SerializeField] private float turnSpeed = 10f;
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float gravityMultiplier = 1f;
        [SerializeField] private float groundDistance = 0.5f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float rotationSpeed = 10;
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float acceptanceRadius = 1f;
        [SerializeField] private Transform chaseTarget; //player target to chase -> TODO: need to set dynamically later.
        [SerializeField] private Transform patrolTarget;

        [Tooltip("Maximum distance for random patrol points")]
        public float roamRadius = 10f;
        [Tooltip("Movement Speed Multiplier")]
        public float speed = 3.5f;
        
        public float originalSpeed { get; set; }
        private CharacterController controller;
        private Animator _animator;
        private Vector3 velocity;
        private bool _canMove = true;
        private NavMeshAgent _agent;
        private int idx = 0; //current patrol point index.
        private Vector3 currentVelocity = Vector3.zero;

        [Header("Flee Settings")]
        [SerializeField] private float zigZagInterval = 1.5f;
        [SerializeField] private float fleeDistance = 10f;
        [SerializeField] private float zigZagAngle = 45f;
        private float _zigZagTimer;
        private int _zigZagDirection = 1;
        private Vector3 _lastFleePosition;
        private float _stuckCheckTimer;
        private float _stuckCheckInterval = 0.5f;
        private float _minimumMoveDistance = 0.1f;

        [Header("Ground Validation")]
        [SerializeField] private float groundCheckHeight = 20f;
        [SerializeField] private float groundCheckDistance = 200f;
        
        
        protected virtual void Awake()
        {
            controller = GetComponent<CharacterController>();
            _agent = GetComponent<NavMeshAgent>();
            
            originalSpeed = patrolSpeed; // cache default speed

            _agent.stoppingDistance = acceptanceRadius;
            _agent.updatePosition = false; //drive position with CharacterController
            _agent.updateRotation = false; //manually control rotation
            _agent.nextPosition = transform.position; // keep agent on track
            _agent.autoBraking = false;
            if (chaseTarget == null) Debug.LogError("No Chase Target");
            if (patrolPoints == null || patrolPoints.Length == 0) Debug.LogError("No Valid Patrol points");
            
            NextPatrolPoint();
            PatrolMovement();

            _agent.angularSpeed = 360f;
            _agent.acceleration = 5f;
        }

        protected virtual void Update()
        {
            if (!IsGrounded())
            {
                velocity.y += gravity * gravityMultiplier * Time.deltaTime;
            }
            else
            {
                velocity.y = -2f; //small value to keep grounded.
            }
            controller.Move(velocity * Time.deltaTime);
        }

        public void PatrolMovement()
        {
            if (!_canMove) return;
            
            //advance to next patrol point if valid
            if (_agent.hasPath && !_agent.pathPending && _agent.remainingDistance <= acceptanceRadius) NextPatrolPoint();
            
            //Validate current destination has ground 
            if (_agent.hasPath && !ValidateGroundAtPosition(_agent.destination))
            {
                //Current destination is in air, find grounded alternatives
                Vector3 groundedPosition = FindGroundedPosition(_agent.destination);
                if (groundedPosition != Vector3.zero)
                {
                    _agent.SetDestination(groundedPosition);
                }
                else
                {
                    NextPatrolPoint();
                    return;
                }
            }
            
            //get desired movement vector(world)
            Vector3 desired = _agent.desiredVelocity;
            //if not yet reached target
            if (desired.sqrMagnitude > 0.01f)
            {
                Quaternion look = Quaternion.LookRotation(desired);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
                
                //SET horizontal velocity, Update() will apply combined veloity
                velocity.x = desired.normalized.x * patrolSpeed;
                velocity.z = desired.normalized.z * patrolSpeed;
            }
            else
            {
                velocity.x = 0f;
                velocity.z = 0f;
            }
            //sync position
            _agent.nextPosition = transform.position;
        }

        public void StopPatrol()
        {
            if (_agent == null) return;
            _agent.isStopped = true;
            _agent.ResetPath();
            velocity.x = 0f;
            velocity.z = 0f;
        }

        private bool ValidateGroundAtPosition(Vector3 position)
        {
            Vector3 rayStart = position + Vector3.up * groundCheckHeight;
            // Remove groundMask temporarily to test
            bool hasGround = Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, groundCheckDistance);
            return hasGround;
        }
        private Vector3 FindGroundedPosition(Vector3 position)
        {
            Vector3 rayStart = position + Vector3.up * groundCheckHeight;

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
            {
                //Sample NavMesh at ground point
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 5f, NavMesh.AllAreas))
                {
                    return navHit.position;
                }
            }
            return Vector3.zero; // no ground found
        }
        
        
        public void NextPatrolPoint()
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;
            _agent.SetDestination(patrolPoints[idx].position);
            idx = (idx + 1) % patrolPoints.Length;
        }

        public void ChaseTarget()
        {
            if (!_canMove) return;
            if (chaseTarget == null) return;

            float distanceToTarget = Vector3.Distance(transform.position, chaseTarget.position);
            if (distanceToTarget <= acceptanceRadius)
            {
                if (_agent.hasPath)
                {
                    _agent.ResetPath();
                }
                velocity.x = 0f;
                velocity.z = 0f;
                //smoothly rotate to face target
                _agent.nextPosition = transform.position;
                return;
            }
            Vector3 targetPosition = chaseTarget.position;

            if (!ValidateGroundAtPosition(targetPosition))
            {
                Vector3 groundedPosition = FindGroundedPosition(targetPosition);
                if (groundedPosition != Vector3.zero)
                {
                    targetPosition = groundedPosition;
                }
                else
                {
                    return;
                }
            }
            
            
            _agent.SetDestination(chaseTarget.position);
            Vector3 desired = _agent.desiredVelocity;
            if (desired.sqrMagnitude > 0.01f)
            {
                Quaternion look = Quaternion.LookRotation(desired);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
                
                //SET horizontal velocity, don't move yet.
                velocity.x = desired.normalized.x * chaseSpeed;
                velocity.z = desired.normalized.z * chaseSpeed;
            }
            else
            {
                velocity.x = 0f;
                velocity.z = 0f;
            }
            _agent.nextPosition = transform.position;
        }

        private void SetFleeDestination()
        {
            if (chaseTarget == null) return;

            Vector3 directionAwayFromPlayer = (transform.position - chaseTarget.position).normalized;
            Vector3 rightOffset = Vector3.Cross(directionAwayFromPlayer, Vector3.up);

            float zigZagScale = _zigZagDirection * Mathf.Tan(zigZagAngle * Mathf.Deg2Rad) * fleeDistance;
            Vector3 zigZagOffset = rightOffset * zigZagScale;
            
            Vector3 fleeTarget = transform.position + (directionAwayFromPlayer * fleeDistance) + zigZagOffset;
            
            //Raycast find Actual Ground
            if (Physics.Raycast(fleeTarget + Vector3.up * 100f, Vector3.down, out RaycastHit hit, 200f, groundMask))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 5f, NavMesh.AllAreas))
                {
                    _agent.SetDestination(navHit.position);
                    return;
                }
            }
            else
            {
                if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit fallbackHit, fleeDistance, NavMesh.AllAreas))
                {
                    _agent.SetDestination(fallbackHit.position);
                }
            }
        }

        public void FleeFromTarget()
        {
            if (!_canMove) return;

            _zigZagTimer += Time.deltaTime;
            _stuckCheckTimer += Time.deltaTime;
            
            //If Reach destination, generate new
            if (_agent.hasPath && !_agent.pathPending && _agent.remainingDistance <= acceptanceRadius)
            {
                _zigZagDirection *= -1;
                SetFleeDestination();
                _zigZagTimer = 0f; // reset when reach destination
            }
            
            //Check if AI Stuck
            if (_stuckCheckTimer >= _stuckCheckInterval)
            {
                float distanceMoved = Vector3.Distance(transform.position, _lastFleePosition);
                if (distanceMoved < _minimumMoveDistance)
                {
                    //Stuck - Generate New Flee Direction
                    _zigZagDirection *= -1;
                    SetFleeDestination();
                }
                _lastFleePosition = transform.position;
                _stuckCheckTimer = 0f;
            }
            
            //Regular zig-zag timer.
            if (_zigZagTimer >= zigZagInterval)
            {
                _zigZagTimer = 0f;
                _zigZagDirection *= -1;
                SetFleeDestination();
            }

            Vector3 desired = _agent.desiredVelocity;
            if (desired.sqrMagnitude > 0.01f)
            {
                Quaternion look = Quaternion.LookRotation(desired);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
                //SET horizontal velocity, don't move yet.
                velocity.x = desired.normalized.x * fleeSpeed;
                velocity.z = desired.normalized.z * fleeSpeed;
            }
            else
            {
                velocity.x = 0f;
                velocity.z = 0f;
            }
            _agent.nextPosition = transform.position;
        }

        public void ResetFleePosition()
        {
            _lastFleePosition = transform.position;
            _stuckCheckTimer = 0f;
            _zigZagTimer = 0f;
            //Set initial flee destination immediately
            SetFleeDestination();
        }
        public bool IsGrounded()
        {
            Vector3 spherePosition = transform.position;
            spherePosition.y -= controller.height / 2f + controller.skinWidth - controller.radius;
            return Physics.CheckSphere(spherePosition, groundDistance, groundMask);
        }
        
        public void Jump()
        {
            if (!IsGrounded() && _canMove) return;
            
            velocity.y = Mathf.Sqrt(jumpHeight * (-2f * gravity));
        }

        public void Knockback(Vector3 attackerPosition, float knockbackForce)
        {
            Vector3 knockbackDirection = (transform.position - attackerPosition).normalized;
            StartCoroutine(KnockbackCoroutine(knockbackDirection,knockbackForce, 0.2f));
        }

        private IEnumerator KnockbackCoroutine(Vector3 knockbackDirection, float knockbackForce, float duration)
        {
            float elapsed = 0f;
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + knockbackDirection * knockbackForce;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                //interpolate
                Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
                controller.Move(newPos - transform.position);
                elapsed += Time.deltaTime;
                yield return null;
            }
            //ensure target position reached.
            controller.Move(targetPos - transform.position);
        }
        
        public void Stop()
        {
            _canMove = false;
            velocity.x = 0f;
            velocity.z = 0f;
            _agent.ResetPath();
        }

        public void Resume()
        {
            _canMove = true;
        }

        public void SetChaseSpeed(float speed)
        {
            chaseSpeed = speed;
        }
    }
}