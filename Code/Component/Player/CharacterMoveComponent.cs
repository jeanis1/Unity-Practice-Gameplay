using System.Collections;
using UnityEngine;

namespace Code.Component
{
    // Segregated interfaces following Interface Segregation Principle
    public interface IMovable
    {
        void Stop();
        bool IsGrounded();
        void Knockback(Vector3 attackerPosition, float knockbackForce);
    }

    public interface IDirectionalMovement
    {
        void DirectionMove(float movex, float movey);
        void RotateToMovement(float movex, float movey);
    }

    public interface IJumpable
    {
        void Jump();
    }

    public interface ISpeedModifiable
    {
        float originalSpeed { get; set; }
        void SetSpeed(float speed);
        void BuffSpeed(float speed);
        void RestoreOriginalSpeed();
    }

    // Legacy interface for backward compatibility
    public interface IMove : IMovable, IDirectionalMovement, IJumpable, ISpeedModifiable
    {
    }

    [RequireComponent(typeof(CharacterInputComponent))]
    [RequireComponent(typeof(CharacterMoveComponent))]
    public class CharacterMoveComponent : MonoBehaviour, IMove, ISpeedReceiver
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float turnSpeed = 10f;
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float gravityMultiplier = 1f;
        [SerializeField] private Transform _meshTransform;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float groundDistance = 0.5f;
        [SerializeField] private LayerMask groundMask;
        public float originalSpeed { get; set; }
        private CharacterController controller;
        private Animator _animator;
        private Vector3 velocity;
        private bool _canMove = true;
        
        void Awake()
        {
            controller = GetComponent<CharacterController>();
            originalSpeed = moveSpeed; //cache default speed to fall back to   
            if (cameraPivot == null && Camera.main != null) cameraPivot = Camera.main.transform;
        }

        void Update()
        {
            
            //stick to ground, avoid gradually drifting
            if (IsGrounded() && velocity.y < 0) velocity.y = -1f;
            
            ApplyGravity();
        }


        public void DirectionMove(float movex, float movey)
        {
            if (!_canMove) return;
            
            //Create local movement 
            Vector3 localInput = new Vector3(movex, 0f, movey);
            //convert local input into world space relative to camera pivot
            Vector3 move = cameraPivot.TransformDirection(localInput);
            move.y = 0f;

            controller.Move(move * (moveSpeed * Time.deltaTime));
            RotateToMovement(movex, movey);

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

        private void ApplyGravity()
        {
            velocity.y += gravity * (gravityMultiplier * Time.deltaTime);
            controller.Move(velocity * Time.deltaTime);
        }

        //Rotate character based on camera's relative position,
        public void RotateToMovement(float movex, float movey)
        {
            if (movex == 0 && movey == 0) return; 
            
            Vector3 camForward = cameraPivot.forward;
            Vector3 camRight = cameraPivot.right;
            camForward.y = 0f;
            camForward.Normalize();
            camRight.y = 0f;
            camRight.Normalize();
            
            //Press "forward" always moves character toward camera's forward.
            Vector3 desiredDirection = camForward * movey + camRight * movex;

            if (desiredDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(desiredDirection, Vector3.up);
                _meshTransform.rotation =
                    Quaternion.Slerp(_meshTransform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }

        public void Knockback(Vector3 attackerPosition, float knockbackForce)
        {
            Vector3 knockbackDirection = (transform.position - attackerPosition).normalized;
            StartCoroutine(KnockbackCoroutine(knockbackDirection, knockbackForce, 0.2f));
        }

        private IEnumerator KnockbackCoroutine(Vector3 knockbackDirection, float knockbackForce, float duration)
        {
            float elapsed = 0f;
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + knockbackDirection * knockbackForce;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
                controller.Move(newPos - transform.position);
                elapsed += Time.deltaTime;
                yield return null;
            }
            //ensure target reached
            controller.Move(targetPos - transform.position);
        }
        
        public void Stop()
        {
            _canMove = false;
        }

        public void SetSpeed(float speed)
        {
            moveSpeed = speed;
        }

        public void BuffSpeed(float speedAmount)
        {
            moveSpeed += speedAmount;
            Debug.Log("Buffed Speed: " + moveSpeed);
        }

        public void RestoreOriginalSpeed()
        {
            moveSpeed = originalSpeed;
            Debug.Log("Reset Speed: " + moveSpeed);

        }
    }
}