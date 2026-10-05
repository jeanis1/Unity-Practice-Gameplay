using UnityEngine;
using Code.Component.AI;

namespace Code.Component.Item
{
    public class DamageItem : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private SphereCollider _collider;
        [SerializeField] private float damage = 50f;
        [SerializeField] private Canvas pickupText;


        void Awake()
        {
            pickupText.enabled = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                pickupText.enabled = true;
                CharacterInteractComponent interact = other.GetComponent<CharacterInteractComponent>();
                if (interact != null)
                {
                    interact.SetOverlappedInteractable(gameObject);
                }
            }

            if (other.CompareTag(enemyTag))
            {
                pickupText.enabled = true;
                CharacterInteractComponent interact = other.GetComponent<CharacterInteractComponent>();
                if (interact != null)
                {
                    interact.SetOverlappedInteractable(gameObject);
                }
            }
        }
        
        
        
        //UnityEngine.Object.Destroy(_owningGameObject);
    }
}
