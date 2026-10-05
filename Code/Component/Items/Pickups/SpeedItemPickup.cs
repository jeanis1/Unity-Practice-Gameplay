using System;
using System.Collections;
using UnityEngine;
using Code.Component.AI;

namespace Code.Component.Item
{
    public class SpeedItemPickup : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private SphereCollider collider;
        [SerializeField] private float SpeedBuffAmount = 5f;
        private float speedBuffDuration = 3f;
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

        }

        private void OnTriggerExit(Collider other)
        {
            pickupText.enabled = false;
            CharacterInteractComponent interact = other.GetComponent<CharacterInteractComponent>();
            if (interact != null)
            {
                interact.RemoveOverlappedInteractable(gameObject);
            }
        }

    }
}
