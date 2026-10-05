using UnityEngine;

namespace Code.Component.Items
{
    public class PickupItem : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        public Item item; 
        
        [Header("Pickup UI")]
        [SerializeField] private Canvas pickupText;
        
        private void Awake()
        {
            if (pickupText != null)
            {
                pickupText.enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                if (pickupText != null)
                {
                    pickupText.enabled = true;
                }
                
                CharacterInteractComponent interact = other.GetComponent<CharacterInteractComponent>();
                if (interact != null)
                {
                    interact.SetOverlappedInteractable(gameObject);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                if (pickupText != null)
                {
                    pickupText.enabled = false;
                }
                
                CharacterInteractComponent interact = other.GetComponent<CharacterInteractComponent>();
                if (interact != null)
                {
                    interact.RemoveOverlappedInteractable(gameObject);
                }
            }
        }

        public void Pickup(GameObject player)
        {
            if (item != null)
            {
                var inventory = player.GetComponent<Player.CharacterInventoryComponent>();
                if (inventory != null)
                {
                    inventory.AddItem(item);
                    Destroy(gameObject);
                }
            }
        }
    }
}
