using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Component
{
    public interface IInput
    {
        float MoveX { get; }
        float MoveY { get; }
        InputAction Jump { get; }
        InputAction Attack1 { get; }
        InputAction Interact { get; }
        InputAction Escape { get; }
        InputAction ToggleInventory { get; }
    }
    public class CharacterInputComponent : MonoBehaviour, IInput
    {
        public Vector2 MouseDelta { get; private set; } //store mouse movement 
        private PlayerInputActions input;
         public float MoveX => input.Player.MoveX.ReadValue<float>();
         public float MoveY => input.Player.MoveY.ReadValue<float>();
         public InputAction Jump => input.Player.Jump;
         public InputAction Attack1 => input.Player.Attack1;
         public InputAction Interact=>input.Player.Interact;
         public InputAction ToggleInventory => input.Player.ToggleInventory;
         public InputAction Escape => input.Player.Esc;
         
         //Camera
         [SerializeField] private Transform cameraPivot;
         [SerializeField] private float mouseSensitivity = 1f;
         [SerializeField] private float minPitch = -90f;
         [SerializeField] private float maxPitch = 90f;
         private float pitch = 0f;
         private float yaw = 0f;
         
         private void Awake()
         {
             input = new PlayerInputActions();
             if (input.Player.ToggleInventory == null)
             {
                 Debug.LogError("Toggle Inventory action not found in PlayerInputActions!", this);
             }
             
         }

         private void Update()
         {
         }

         private void LateUpdate()
         {
             RotateCameraToMouse();

         }

         void RotateCameraToMouse()
         {
             Vector2 mouseDelta = Mouse.current.delta.ReadValue();

             yaw += mouseDelta.x * mouseSensitivity * Time.deltaTime;
             pitch -= mouseDelta.y * mouseSensitivity * Time.deltaTime;
             
             pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
             if (cameraPivot) cameraPivot.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
         }
         


          private void OnEnable() => input.Enable();
          private void OnDisable() => input.Disable();
     }

}

