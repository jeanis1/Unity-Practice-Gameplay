using Code.Component.Items;
using Code.Component.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.UI.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private CharacterInventoryComponent characterInventory;
        [SerializeField] private InputActionReference toggleInventoryAction;
        [SerializeField] private ItemDescriptionUI itemDescriptionUI;
        [SerializeField] private int startInventoryIndex = 10;
        
        private ItemSlotUI[] itemSlots;
        private bool isInventoryOpen = false;

        private void OnEnable()
        {
            toggleInventoryAction.action.Enable();
            toggleInventoryAction.action.performed += OnToggleInventory;
        }

        private void OnDisable()
        {
            toggleInventoryAction.action.performed -= OnToggleInventory;
            toggleInventoryAction.action.Disable();
        }
        
        private void Start()
        {
            InitializeItemSlots();
            
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            if (characterInventory != null)
            {
                characterInventory.OnInventoryChanged += RefreshInventoryUI;
            }

            if (InventoryDragHandler.Instance != null)
            {
                InventoryDragHandler.Instance.SetInventoryComponent(characterInventory);
            }
            SetCursorState(false);
        }

        private void InitializeItemSlots()
        {
            if (inventoryPanel == null)
            {
                Debug.LogError("inventoryPanel is null");
                return;
            }
            
            itemSlots = inventoryPanel.GetComponentsInChildren<ItemSlotUI>(true);

            if (itemSlots.Length == 0)
            {
                Debug.LogError("No ItemSlotUI components found in InventoryPanel children.", this);
            }
            else
            {
                for (int i = 0; i < itemSlots.Length; i++)
                {
                    itemSlots[i].SetInventoryComponent(characterInventory);
                    itemSlots[i].SetSlotIndex(startInventoryIndex + i);

                    if (itemDescriptionUI != null)
                    {
                        itemSlots[i].SetItemDescriptionUI(itemDescriptionUI);
                    }
                }
            }
        }
        
        private void OnDestroy()
        {
            if (characterInventory != null)
            {
                characterInventory.OnInventoryChanged -= RefreshInventoryUI;
            }
        }

        private void OnToggleInventory(InputAction.CallbackContext context)
        {
            ToggleInventory();
        }

        public void ToggleInventory()
        {
            isInventoryOpen = !isInventoryOpen;
            
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(isInventoryOpen);
            }

            SetCursorState(isInventoryOpen);
            if (isInventoryOpen)
            {
                RefreshInventoryUI();
            }
            else
            {
                //Hide Description when inventory close
                if (itemDescriptionUI != null)
                {
                    itemDescriptionUI.Hide();
                }
            }
        }
        
        private void SetCursorState(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void RefreshInventoryUI()
        {
            if (characterInventory == null)
            {
                Debug.LogWarning("CharacterInventory is null, cannot refresh UI!");
                return;
            }

            var items = characterInventory.ListInventory();
            
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i] == null) continue;

                int inventoryIndex = startInventoryIndex + i;
                
                if (inventoryIndex < items.Count && items[inventoryIndex] != null)
                {
                    itemSlots[i].SetItem(items[inventoryIndex]);
                }
                else
                {
                    itemSlots[i].ClearSlot();
                }
            }
        }
    }
}

