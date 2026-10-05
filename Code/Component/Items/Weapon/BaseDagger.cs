using System.Collections.Generic;
using UnityEngine;
using Code.Component;
using Code.Component.AI;
using Code.Weapon;
using JetBrains.Annotations;
using NUnit.Framework.Interfaces;
using TMPro;


namespace Code.Weapon
{
    public class BaseDagger : MonoBehaviour, IWeapon
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private CapsuleCollider hitboxCollider;
        [SerializeField] private CapsuleCollider equipCollider;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float amplitude = 0.5f;
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float hoverHeight = 0f;
        [SerializeField] private Canvas equipText;

        private bool _equipped = false;
        private Vector3 _startPosition;
        private Collider _ownerCollider;
        public float damageAmt { get; set; } = 0f;
        private HashSet<GameObject> _hitTargets = new HashSet<GameObject>();
        private HashSet<Collider> _overlappingColliders = new HashSet<Collider>();
        
        private bool _damageEnabled = false;
        public bool damageEnabled
        {
            get => _damageEnabled;
            set
            {
                _damageEnabled = value;
                if (hitboxCollider != null) hitboxCollider.enabled = value;
                if (value) SweepForTargets();
                if (!value) _hitTargets.Clear();
            }
        }

        private void SweepForTargets()
        {
            Collider[] hits = Physics.OverlapBox(
                hitboxCollider.bounds.center,
                hitboxCollider.bounds.extents,
                hitboxCollider.transform.rotation);

            foreach (Collider hit in hits)
            {
                if (hit != hitboxCollider) TryApplyDamage(hit);
            }
        }

        void Awake()
        {
            _startPosition = transform.position;
            if (!hitboxCollider || !equipCollider || !rb)
            {
                Debug.LogError("BaseDagger missing references!");
                return;
            }
            equipText.enabled = false;
            if (hitboxCollider != null) hitboxCollider.enabled = false;

        }

        void Update()
        {
            if (!_equipped) SinusoidalHover();
        }
        private void SinusoidalHover()
        {
            float yOffset = amplitude * Mathf.Sin(Time.time * frequency * Mathf.PI * 2);
            transform.position = new Vector3(_startPosition.x, _startPosition.y + yOffset, _startPosition.z);
        }

        private void TryApplyDamage(Collider other)
        {
            if (!_equipped || !damageEnabled) return;

            GameObject target = other.transform.root.gameObject;
            if (_hitTargets.Contains(target)) return;
            _hitTargets.Add(target);
            if (target.CompareTag(playerTag) && other != _ownerCollider)
            {
                IDamageable controller = target.GetComponent<IDamageable>();
                if (controller != null) controller.TakeDamage(transform.position, damageAmt);
            }
            else if (target.CompareTag(enemyTag))
            {
                IAI_Enemy controller = target.GetComponent<IAI_Enemy>();
                if (controller != null) controller.TakeDamage(transform.position, damageAmt);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            _overlappingColliders.Add(other);
            
            if (other.CompareTag(playerTag) && !_equipped)
            {
                equipText.enabled = true;
                CharacterInteractComponent interact = other.GetComponent<CharacterInteractComponent>();
                if (interact != null)
                {
                    interact.SetOverlappedInteractable(gameObject);
                }
                TryApplyDamage(other);
            }

            
            //Damage
            if (_equipped && damageEnabled)
            {
                GameObject target = other.transform.root.gameObject;
                if (_hitTargets.Contains(target)) return;
                _hitTargets.Add(target);

                if (other.CompareTag(playerTag) && other != _ownerCollider && other.GetComponent<CharacterControllerComponent>())
                {
                    IDamageable controller = other.GetComponent<IDamageable>();
                    Vector3 attackDirection = transform.position;
                    controller.TakeDamage(attackDirection, damageAmt);
                }

                if (other.CompareTag(enemyTag) && other.GetComponent<AI_ControllerComponent>())
                {
                    IAI_Enemy controller = other.GetComponent<AI_ControllerComponent>();
                    Vector3 attackDirection = transform.position;
                    controller.TakeDamage(attackDirection, damageAmt);
                }
            }
            //Checks if weapon attack is activated - TODO needs update
            //Need to call Apply Damage to Player
        }

        private void OnTriggerStay(Collider other)
        {
            TryApplyDamage(other);
        }
        
        private void OnTriggerExit(Collider other)
        {
            _overlappingColliders.Remove(other);
            
            if (other.CompareTag(playerTag))
            {
                equipText.enabled = false;

                CharacterInteractComponent interact = other.GetComponent<CharacterInteractComponent>();
                if (interact != null)
                {
                    interact.SetOverlappedInteractable(gameObject);
                }
            }
        }

        public void Equip(Transform rightHandPoint, Transform leftHandPoint, GameObject parent)
        {
            rb.isKinematic = true;
            equipCollider.enabled = false;
            //Need to Attach to correct hand - depends on animation & weapon
            transform.SetParent(leftHandPoint, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            gameObject.SetActive(true);
            _ownerCollider = parent.GetComponent<Collider>();
            if (_ownerCollider == null)
            {
                Debug.LogError("BaseDagger: Parent has no collider component!", parent);
            }
            _equipped = true; //disable sinusoidal movement 
            equipText.enabled = false;
        }

        public void Unequip()
        {
            transform.SetParent(null);
            rb.isKinematic = false;
            equipCollider.enabled = false;
            _equipped = false; //re-enable sinusoidal movement
            Vector3 currentPosition = transform.position;
            RaycastHit hit;
            if (Physics.Raycast(currentPosition, Vector3.down, out hit))
            {
                //hover again
                _startPosition = new Vector3(currentPosition.x, currentPosition.y + hoverHeight, currentPosition.z);
            }
            // no ground, reset to original position
            else _startPosition = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);

            equipText.enabled = true;
        }

        public void SetWeaponType(IStatus status)
        {
            status.WeaponType = EquippedWeaponType.BaseDagger;
            Debug.Log("Base Dagger set as Weapon Type.");
        }
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}


