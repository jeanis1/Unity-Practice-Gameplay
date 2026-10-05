using UnityEngine;

namespace Code.Component.AI
{
    public class AI_DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private Canvas interactText;
        [SerializeField] private string playerTag = "Player";
        private bool interacted = false;

        void Awake()
        {
            interactText.enabled = false;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag) && !interacted)
            {
                interactText.enabled = true;
                CharacterInteractComponent interact = other.GetComponent<CharacterInteractComponent>();
                if (interact != null)
                {
                    //set interact Target
                    interact.SetOverlappedInteractable(gameObject);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                interactText.enabled = false;
            }
            CharacterInteractComponent interact = other.GetComponent<CharacterInteractComponent>();
            if (interact != null)
            {
                interact.RemoveOverlappedInteractable(gameObject);
            }
        }
    }
}
