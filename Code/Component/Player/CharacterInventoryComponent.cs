using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Code.Component.Player
{
    public class CharacterInventoryComponent : MonoBehaviour
    {
        [FormerlySerializedAs("_inventory")]
        [SerializeField] private List<Items.Item> inventory = new List<Items.Item>();
        public event System.Action OnInventoryChanged;
        private IStatus _status;


        public bool AddItem(Items.Item item)
        {
            if (!item) return false;

            //Clean up previous null entries only after index 10 (preserve hotbar structure)
            for (int i = inventory.Count - 1; i >= 10; i--)
            {
                if (inventory[i] == null)
                {
                    inventory.RemoveAt(i);
                }
            }
            
            if (item.MaxStack > 1)
            {
                //Find matching item with room in stack (inventory, not hotbar)
                for (int i = 10; i < inventory.Count; i++)
                {
                    if (inventory[i] != null &&
                        inventory[i].ItemName == item.ItemName &&
                        inventory[i].CurrentStack < inventory[i].MaxStack)
                    {
                        inventory[i].CurrentStack++;
                        OnInventoryChanged?.Invoke();
                        return true;
                    }
                }
            }
            while (inventory.Count < 10)
            {
                inventory.Add(null);
            }
            
            inventory.Add(item);
            OnInventoryChanged?.Invoke();
            return true;
        }

        public List<Items.Item> ListInventory()
        {
            return inventory;
        }
        
        public bool UpdateItem(Items.Item item)
        {
            if (!item) return false;
            int index = inventory.FindIndex(e => e.ItemName == item.ItemName);
            if (index != -1)
            {
                inventory[index] = item;
                OnInventoryChanged?.Invoke();
                return true;
            }
            return false;
        }
        
        public bool DeleteItem(Items.Item item)
        {
            if (!item) return false;
            inventory.Remove(item);            
            OnInventoryChanged?.Invoke();
            return true;
        }
        
        public bool DropFromInventory(Items.Item item)
        {
            if (!item) return false;
            //TODO - Re-Instantiate Prefab & Delete from Inventory
            inventory.Remove(item);
            OnInventoryChanged?.Invoke();
            return true;
        }
        
        public void UseItem(Items.Item item)
        {
            if (!item) return;
            item.Use(gameObject);
            
            // Only removable consumables from inventory
            //Weapons stay in inventory after equip (until Equip UI implemented)
            if (item.ItemType != Items.ItemType.Weapon)
            {
                if (item.MaxStack > 1 && item.CurrentStack > 1)
                {
                    item.CurrentStack--;
                }
                else
                {
                    // Last item in stack or non-stackable - remove from inventory
                    inventory.Remove(item);
                }
            }
            OnInventoryChanged?.Invoke();
        }

        public void NotifyInventoryChanged()
        {
            OnInventoryChanged?.Invoke();
        }

        public void EmptyInventory()
        {
            if (inventory.Count == 0) return;
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] != null) continue;
                inventory.RemoveAt(i);
                Debug.Log("Empty Inventory loop called.");
            }
            OnInventoryChanged?.Invoke();
        }
        
    }
}