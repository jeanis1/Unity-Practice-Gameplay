using Code.Component.Items;
using Code.Component.Player;
using UnityEngine;

namespace Code.UI.Inventory
{
    public class HotbarUI : MonoBehaviour
    {
        [SerializeField] private GameObject hotbarPanel;
        [SerializeField] private CharacterInventoryComponent characterInventory;
        [SerializeField] private ItemDescriptionUI itemDescriptionUI;
        
        private ItemSlotUI[] hotbarSlots;

        private void Start()
        {
            InitializeHotbarSlots();
            
            if (hotbarPanel != null)
            {
                hotbarPanel.SetActive(true);
            }
            
            if (characterInventory != null)
            {
                characterInventory.OnInventoryChanged += RefreshHotbarUI;
                RefreshHotbarUI();
            }

            if (InventoryDragHandler.Instance != null)
            {
                InventoryDragHandler.Instance.SetInventoryComponent(characterInventory);
            }
        }

        private void OnDestroy()
        {
            if (characterInventory != null)
            {
                characterInventory.OnInventoryChanged -= RefreshHotbarUI;
            }
        }
        
        private void InitializeHotbarSlots()
        {
            hotbarSlots = hotbarPanel.GetComponentsInChildren<ItemSlotUI>(true);

            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                hotbarSlots[i].SetInventoryComponent(characterInventory);
                hotbarSlots[i].SetSlotIndex(i);

                if (itemDescriptionUI != null)
                {
                    hotbarSlots[i].SetItemDescriptionUI(itemDescriptionUI);
                }
            }
        }

        private void RefreshHotbarUI()
        {
            if (characterInventory == null) return;
            
            var items = characterInventory.ListInventory();

            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                if (i < items.Count && items[i] != null)
                {
                    hotbarSlots[i].SetItem(items[i]);
                }
                else
                {
                    hotbarSlots[i].ClearSlot();
                }
            }
        }
    }
}