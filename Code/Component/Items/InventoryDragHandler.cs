using UnityEngine;
using UnityEngine.UI;
using Code.Component.Player;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Code.Component.Items
{
    public class InventoryDragHandler : MonoBehaviour
    {
        public static InventoryDragHandler Instance { get; private set; }

        [SerializeField] private Canvas canvas;
        [SerializeField] private Image draggingIcon;
        [SerializeField] private float dropDistance = 2f;
        [SerializeField] private int hotbarSlotCount = 10;
        
        private Item selectedItem;
        private ItemSlotUI sourceSlot;
        private CharacterInventoryComponent inventoryComponent;
        private GameObject playerObject;
        private bool clickHandledBySlot;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            if (draggingIcon != null)
            {
                draggingIcon.enabled = false;
            }
        }

        private void Update()
        {
            if (selectedItem != null && draggingIcon != null)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Vector2 position;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    mousePosition,
                    canvas.worldCamera,
                    out position
                );
                draggingIcon.transform.position = canvas.transform.TransformPoint(position);
            }
        }

        private void LateUpdate()
        {
            if (selectedItem != null && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (!clickHandledBySlot && !IsPointerOverUIElement())
                {
                    DropItem();
                }
            }
            clickHandledBySlot = false;
        }

        private bool IsPointerOverUIElement()
        {
            if (EventSystem.current == null) return false;
            
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = mousePosition
            };
            
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            foreach (var result in results)
            {
                if (result.gameObject.GetComponent<ItemSlotUI>() != null)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetInventoryComponent(CharacterInventoryComponent component)
        {
            inventoryComponent = component;
            playerObject = component.gameObject;
        }

        private void DropItem()
        {
            if (selectedItem == null || playerObject == null) return;
    
            if (selectedItem.ModelPrefab != null)
            {
                Vector3 forwardDirection = -playerObject.transform.forward;
                forwardDirection.y = 0;
                forwardDirection.Normalize();

                Vector3 dropPosition = playerObject.transform.position + forwardDirection * dropDistance;
                GameObject droppedObject = Instantiate(selectedItem.ModelPrefab, dropPosition, Quaternion.identity);
                droppedObject.SetActive(true);
        
                PickupItem pickupComponent = droppedObject.GetComponent<PickupItem>();
                if (pickupComponent != null)
                {
                    pickupComponent.item = selectedItem;
                }
            }
            inventoryComponent.DeleteItem(selectedItem);
            ClearSelection();
        }
        
        public void OnSlotClicked(ItemSlotUI clickedSlot, bool isShiftHeld)
        {
            clickHandledBySlot = true;
            
            if (isShiftHeld && clickedSlot.Item != null)
            {
                QuickMoveItem(clickedSlot);
                return;
            }
            
            if (selectedItem == null)
            {
                if (clickedSlot.Item != null)
                {
                    SelectItem(clickedSlot);
                }
            }
            else
            {
                PlaceItem(clickedSlot);
            }
        }
        
        private void QuickMoveItem(ItemSlotUI clickedSlot)
        {
            var inventory = inventoryComponent.ListInventory();
            int sourceIndex = clickedSlot.SlotIndex;
            bool isFromHotbar = sourceIndex < hotbarSlotCount;
            
            int targetIndex = -1;
            
            if (isFromHotbar)
            {
                for (int i = hotbarSlotCount; i < inventory.Count; i++)
                {
                    if (inventory[i] == null)
                    {
                        targetIndex = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < hotbarSlotCount && i < inventory.Count; i++)
                {
                    if (inventory[i] == null)
                    {
                        targetIndex = i;
                        break;
                    }
                }
            }
            
            if (targetIndex != -1)
            {
                SwapItemsInInventory(sourceIndex, targetIndex);
            }
        }
        
        private void SelectItem(ItemSlotUI slot)
        {
            selectedItem = slot.Item;
            sourceSlot = slot;

            if (draggingIcon != null)
            {
                draggingIcon.sprite = selectedItem.Icon;
                draggingIcon.enabled = true;
            }
        }
        
        private void PlaceItem(ItemSlotUI destinationSlot)
        {
            if (destinationSlot.Item == null)
            {   
                SwapItemsInInventory(sourceSlot.SlotIndex, destinationSlot.SlotIndex);
                ClearSelection();
            }
            else
            {
                Item tempItem = destinationSlot.Item;
                SwapItemsInInventory(sourceSlot.SlotIndex, destinationSlot.SlotIndex);

                sourceSlot = destinationSlot;
                selectedItem = tempItem;

                if (draggingIcon != null)
                {
                    draggingIcon.sprite = selectedItem.Icon;
                }
            }
        }
        
        private void SwapItemsInInventory(int fromIndex, int toIndex)
        {
            var inventory = inventoryComponent.ListInventory();
            while (inventory.Count <= Mathf.Max(fromIndex, toIndex))
            {
                inventory.Add(null);
            }
            
            (inventory[toIndex], inventory[fromIndex]) = (inventory[fromIndex], inventory[toIndex]);
            inventoryComponent.NotifyInventoryChanged();
        }

        private void ClearSelection()
        {
            selectedItem = null;
            sourceSlot = null;
            if (draggingIcon != null)
            {
                draggingIcon.enabled = false;
            }
        }
    }
}