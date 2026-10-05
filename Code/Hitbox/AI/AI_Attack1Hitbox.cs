using UnityEngine;
using Code.Component;

namespace Code.Hitbox.AI
{
    public class AI_Attack1Hitbox : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider attack1Collider;
        [SerializeField] private string playerTag = "Player";
        private Collider OwnerCollider;
        public float damageAmt = 10f;


        void Awake()
        {
            if (attack1Collider == null)
            {
                Debug.LogError("Attack1 Collider missing references.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag) && other != OwnerCollider && other.GetComponent<CharacterControllerComponent>())
            {
                IDamageable controller = other.GetComponent<IDamageable>();
                Vector3 direction = transform.position;
                controller.TakeDamage(direction, damageAmt);
            }
        }
    }
}
