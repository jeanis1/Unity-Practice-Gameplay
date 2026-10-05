using System.Collections.Generic;
using UnityEngine;
using Code.Component;
using Code.Component.AI;
using UnityEngine.Serialization;


namespace Code.Weapon
{
    public interface IWeapon
    {
        void Equip(Transform rightHandPoint, Transform leftHandPoint, GameObject parent);
        void Unequip();
        bool damageEnabled { get; set; }
        float damageAmt { get; set; }
        void SetActive(bool active);
        void SetWeaponType(IStatus status);
    }

    public class BaseWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private string enemyTag = "Enemy";
        [FormerlySerializedAs("weaponCollider")]
        [SerializeField] private Collider equipCollider; //pickup/equip only
        [SerializeField] private Collider hitboxCollider;
        [SerializeField] private Rigidbody weaponRigidbody;
        [SerializeField] private float amplitude = 0.5f;
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float hoverHeight = 0f;
        [SerializeField] private Canvas equipText;
        
        private bool equipped = false;
        private Vector3 startPosition;
        private Collider OwnerCollider;
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
            startPosition = transform.position;
            if (equipCollider == null || weaponRigidbody == null) Debug.LogError("BaseWeapon missing references!");
            equipText.enabled = false;

            if (hitboxCollider != null) hitboxCollider.enabled = false;
            
        }

        void Update()
        {
            if (!equipped) SinusoidalHover();
        }

        private void SinusoidalHover()
        {
            float yOffset = amplitude * Mathf.Sin(Time.time * frequency * Mathf.PI * 2);
            transform.position = new Vector3(startPosition.x, startPosition.y + yOffset, startPosition.z);
        }



        private void OnTriggerEnter(Collider other)
        {
            _overlappingColliders.Add(other);
            
            if (other.CompareTag(playerTag) && !equipped)
            {
                equipText.enabled = true;
                CharacterInteractComponent interact = other.GetComponent<CharacterInteractComponent>();
                if (interact != null)
                {
                    interact.SetOverlappedInteractable(gameObject); // set as interact target
                }
            }
            TryApplyDamage(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TryApplyDamage(other);
        }

        private void TryApplyDamage(Collider other)
        {
            if (!equipped || !damageEnabled) return;
            
            GameObject target = other.transform.root.gameObject;
            if (_hitTargets.Contains(target)) return;
            _hitTargets.Add(target);
            
            if (target.CompareTag(playerTag) && other != OwnerCollider)
            {
                IDamageable controller = target.GetComponent<IDamageable>();
                if (controller != null) controller.TakeDamage(transform.position, damageAmt);
            }
            else if (target.CompareTag(enemyTag))
            {
                IAIEnemy controller = target.GetComponent<IAIEnemy>();
                if(controller != null) controller.TakeDamage(transform.position, damageAmt);
            }
            
        }
        
        
        private void OnTriggerExit(Collider other)
        {
            _overlappingColliders.Remove(other);
            
            if (other.CompareTag(playerTag))
            {
                equipText.enabled = false;
            }

            CharacterInteractComponent  interact = other.GetComponent<CharacterInteractComponent>();
            if (interact != null)
            {
                interact.RemoveOverlappedInteractable(gameObject); //clear interactTarget
            }
        }
        
        public void Equip(Transform rightHandPoint, Transform leftHandPoint, GameObject parent)
        {
            weaponRigidbody.isKinematic = true;

            equipCollider.enabled = false;
            
            //Need to Attach to correct hand (depends on animation & weapon)
            transform.SetParent(rightHandPoint, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            OwnerCollider = parent.GetComponent<Collider>();
            equipped = true; //disable sinusoidal movement.
            equipText.enabled = false;
        }

        public void Unequip()
        {
            transform.SetParent(null);
            weaponRigidbody.isKinematic = false;
            equipCollider.enabled = false;
            equipped = false; //re-enable sinusoidal movement
            Vector3 currentPosition = transform.position;
            RaycastHit hit;
            if (Physics.Raycast(currentPosition, Vector3.down, out hit))
            {
                //hover above where unequipped
                startPosition = new Vector3(currentPosition.x, currentPosition.y + hoverHeight, currentPosition.z);
            }
            // no ground detected - fallback to original position
            else startPosition = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);

            equipText.enabled = true;
        }

        public virtual void SetWeaponType(IStatus status)
        {
            status.WeaponType = EquippedWeaponType.BaseSword;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
