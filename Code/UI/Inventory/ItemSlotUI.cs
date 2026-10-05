using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Code.Component.Player;
using Code.UI.Inventory;
using TMPro;
using UnityEngine.InputSystem;

namespace Code.Component.Items
{
    public class ItemSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMP_Text stackText;
        private ItemDescriptionUI itemDescriptionUI;
        
        private Item item;
        private CharacterInventoryComponent inventoryComponent;
        private int slotIndex;
        private RectTransform rectTransform;
        
        public Item Item => item;
        public int SlotIndex => slotIndex;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        public void SetInventoryComponent(CharacterInventoryComponent component)
        {
            inventoryComponent = component;
        }

        public void SetSlotIndex(int index)
        {
            slotIndex = index;
        }

        public void SetItemDescriptionUI(ItemDescriptionUI descriptionUI)
        {
            itemDescriptionUI = descriptionUI;
        }
        
        public void SetItem(Item newItem)
        {
            item = newItem;
            if (item != null)
            {
                itemIcon.sprite = item.Icon;
                itemIcon.enabled = true;
            
                // Display stack count only if stackable and more than 1
                if (item.MaxStack > 1)
                {
                    stackText.text = item.CurrentStack.ToString();
                }
                else
                {
                    stackText.text = "";
                }
            }
            else
            {
                ClearSlot();
            }
        }
        
        public void ClearSlot()
        {
            item = null;
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            stackText.text = "";
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                bool isShiftHeld = Keyboard.current.shiftKey.isPressed;
                InventoryDragHandler.Instance.OnSlotClicked(this, isShiftHeld);
            }
            else if (eventData.button == PointerEventData.InputButton.Right && item != null)
            {
                inventoryComponent.UseItem(item);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (item != null && itemDescriptionUI != null)
            {
                itemDescriptionUI.Show(item, rectTransform);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (itemDescriptionUI != null)
            {
                itemDescriptionUI.Hide();
            }
        }
    }
}
