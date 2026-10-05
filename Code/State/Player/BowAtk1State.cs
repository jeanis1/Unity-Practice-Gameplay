using System.Threading.Tasks;
using Code.Component;
using UnityEngine;
using Code.State;
using Code.Weapon;

namespace Code.State.Player
{
    public class BowAtk1State : IState
    {
        private CharacterControllerComponent _controller;
        private IInteract _interact;
        private IAnimator _animator;
        private ISound _sound;
        private Transform _bowTransform; // Location of Bow when attack called
        private Transform _visualTransform; // Transform for character's visual facing direction
        private float spawnOffset = 0.5f; // Offset in front of bow to spawn arrow
        private float enableAttackDelay = 0.45f; // Sync with animation
        private float enableAttackTimer;
        private float attackDuration = 0.7f;
        private float attackTimer;
        private float _damageAmount;
        private float soundDelay = 0.4f;
        private float soundTimer;
        private bool atkSoundPlayed = false;
        private bool projectileSpawned = false;
        private GameObject _arrowPrefab;
        [SerializeField] private Vector3 _arrowRotationOffset = new Vector3(0,0,0); // Adjustable offset for arrow prefab orientation
        private float _bowRotationOffset = 20f; // New: Rotation offset for bow (20 degrees left, counterclockwise)

        public BowAtk1State(CharacterControllerComponent controller, IInteract interact, IAnimator animator, ISound sound, GameObject arrowPrefab, float damageAmount)
        {
            this._controller = controller;
            this._interact = interact;
            this._animator = animator;
            this._sound = sound;
            _arrowPrefab = arrowPrefab;
            this._damageAmount = damageAmount;
        }

        public void Enter()
        {
            attackTimer = 0f;
            enableAttackTimer = 0f;
            atkSoundPlayed = false;
            soundTimer = 0f;
            projectileSpawned = false;

            // Find the bow by tag "Weapon"
            _bowTransform = FindChildWithTag(_controller.transform, "Weapon");
            
            // Try to find a visual transform (e.g., character model or rig)
            _visualTransform = FindRotatingChild(_controller.transform) ?? _controller.transform;

            // Apply 20-degree rotation offset to the bow (counterclockwise, Z-axis for 2D)
            if (_bowTransform != null)
            {
                // Store current rotation and add offset
                Vector3 currentRotation = _bowTransform.rotation.eulerAngles;
                _bowTransform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z + _bowRotationOffset);
            }

            // Play the animation after setting rotation
            _animator?.Play("BowShoot1");


        }

        public void Execute()
        {
            enableAttackTimer += Time.deltaTime;
            attackTimer += Time.deltaTime;
            soundTimer += Time.deltaTime;

            if (attackTimer >= enableAttackDelay && !projectileSpawned)
            {
                ShootArrow();
                projectileSpawned = true;
            }
            if (soundTimer >= soundDelay && !atkSoundPlayed)
            {
                atkSoundPlayed = true;
                _sound.PlaySound("DrawBow");
            }

            if (attackTimer >= attackDuration)
            {
                _controller.BaseStateMachine.ChangeState(_controller.IdleState);
            }
        }

        public void Exit()
        {
            _interact.DisableWeaponHit();
        }

        private void ShootArrow()
        {
            if (_bowTransform == null || _controller == null || _visualTransform == null)
            {
                Debug.LogError("Bow Transform, Controller, or Visual Transform is null");
                return;
            }

            // Use character's visual forward direction, flattened to XZ plane
            Vector3 forwardDirection = _visualTransform.forward;
            forwardDirection.y = 0f; // Remove Y-component for horizontal trajectory
            forwardDirection = forwardDirection.normalized;

            Vector3 spawnPosition = _bowTransform.position + forwardDirection * spawnOffset;
            Quaternion spawnRotation = Quaternion.LookRotation(forwardDirection) * Quaternion.Euler(_arrowRotationOffset);

            GameObject arrow = GameObject.Instantiate(_arrowPrefab, spawnPosition, spawnRotation);
            IBaseArrow baseArrow = arrow.GetComponent<IBaseArrow>();
            if (baseArrow != null) baseArrow.SetDamage(_damageAmount);
        }

        // Helper method to find a child GameObject by tag
        private Transform FindChildWithTag(Transform parent, string tag)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child.CompareTag(tag))
                {
                    return child;
                }
            }
            return null;
        }

        // Helper method to find a rotating child transform
        private Transform FindRotatingChild(Transform parent)
        {
            // Try common names for visual models or rigs
            string[] possibleNames = { "Model", "Rig", "Character", "Body", "Mesh", "Armature", "PlayerModel", "CharacterMesh", "HumanM_BodyMesh" };
            foreach (string name in possibleNames)
            {
                Transform child = parent.Find(name);
                if (child != null)
                {
                    if (child != parent)
                    {
                        return child;
                    }
                }
            }
            // Try finding a child with a SkinnedMeshRenderer or Animator
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child.GetComponent<SkinnedMeshRenderer>() != null || child.GetComponent<Animator>() != null)
                {
                    return child;
                }
            }
            return null;
        }
    }
}