using UnityEngine;
using Code.Component;
using Code.Component.AI;
using Code.Component.Item;


namespace Code.Weapon
{
    public class BaseBow : MonoBehaviour, IWeapon
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private CapsuleCollider weaponCollider;
        [SerializeField] private Rigidbody weaponRigidbody;
        [SerializeField] private float amplitude = 0.5f;
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float hoverHeight = 0f;
        [SerializeField] private Canvas equipText;

        private bool equipped = false;
        private Vector3 startPosition;
        private Collider OwnerCollider;
        public bool damageEnabled { get; set; }
        public float damageAmt { get; set; }

        void Awake()
        {
            startPosition = transform.position;
            if (weaponCollider == null || weaponRigidbody == null) Debug.LogError("BaseWeapon missing references!");
            equipText.enabled = false;
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
            if (other.CompareTag(playerTag) && !equipped)
            {
                equipText.enabled = true;

                CharacterInteractComponent interact = other.GetComponent<CharacterInteractComponent>();
                if(interact != null)
                {
                    interact.SetOverlappedInteractable(gameObject);
                }


            }

            // MELEE Bow Damage
            // if (equipped && damageEnabled)
            // {
            //     if (other.CompareTag(playerTag) && other != OwnerCollider && other.GetComponent<CharacterControllerComponent>() != null)
            //     {
            //         IPlayer controller = other.GetComponent<IPlayer>();
            //         Vector3 attackDirection = transform.position;
            //         controller.TakeDamage(attackDirection, damageAmt);
            //     }
            //
            //     if (other.CompareTag(enemyTag) && other.GetComponent<AI_ControllerComponent>() != null)
            //     {
            //         IAIEnemy controller = other.GetComponent<IAIEnemy>();
            //         Vector3 attackDirection = transform.position;
            //         controller.TakeDamage(attackDirection, damageAmt);
            //     }
            // }
        }
        private void OnTriggerExit(Collider other)
        {
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
            weaponRigidbody.isKinematic = true;
            weaponCollider.enabled = false;
            //attack to correct hand 
            transform.SetParent(leftHandPoint, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0f, 180f, 0f); //offset for correct bow rotation
            gameObject.SetActive(true);
            OwnerCollider = parent.GetComponent<Collider>();
            equipped = true; // disable sinusoidal movement
            equipText.enabled = false;
        }

        public void Unequip()
        {
            transform.SetParent(null);
            weaponRigidbody.isKinematic = false;
            weaponCollider.enabled = false;
            equipped = false; //enable sinusoidal movement
            Vector3 currentPosition = transform.position;
            RaycastHit hit;
            if (Physics.Raycast(currentPosition, Vector3.down, out hit))
            {
                //hover at unequipped location
                startPosition = new Vector3(currentPosition.x, currentPosition.y + hoverHeight, currentPosition.z);
            }
            //ground not detected, fall back to original location
            else startPosition = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);

            equipText.enabled = true;
        }

        // public void EnableDamage(float damage)
        // {
        //     damageEnabled = true;
        //     damageAmt = damage;
        // }

        public virtual void SetWeaponType(IStatus status)
        {
            status.WeaponType = EquippedWeaponType.BaseBow;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}

