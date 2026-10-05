using UnityEngine;

namespace Code.HUD
{
    public class ItemHUD : MonoBehaviour
    {
        //update Look rotation to camera relative positioning 
        private void Update()
        {
            Quaternion rotation = Camera.main.transform.rotation;
            transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
        }
    }
}