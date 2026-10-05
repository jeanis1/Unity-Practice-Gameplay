using UnityEngine;

namespace Code.Component.Player
{
    public class PreventCharacterStacking : MonoBehaviour
    {
        private CharacterController controller;

        void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //check if Hit another charactercontroller 
            CharacterController otherController = hit.gameObject.GetComponentInParent<CharacterController>();

            if (otherController != null)
            {
                Vector3 pushDirection = hit.transform.position - transform.position;
                pushDirection.y = 0;
                pushDirection.Normalize();

                float pushForce = 5f;
                otherController.Move(pushDirection * pushForce * Time.deltaTime);
            }
        }
        
    }
}
