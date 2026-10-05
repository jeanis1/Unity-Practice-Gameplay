using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Component.Items
{

    public enum ItemType
    {
        None,
        Item,
        Weapon,
        Currency
    }

    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Items")]
    public class Item : ScriptableObject, IUsable
    {
        [SerializeField] private string itemName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject modelPrefab; // Actual In Game Object / Prefab
        [SerializeField] private int maxStack = 1; //stackable items like currency, ammo, etc.
        [SerializeField] private int currentStack = 1;// current QTY of item count
        [FormerlySerializedAs("equipSlot")]
        [SerializeField] private ItemType itemType; //Enum where it can be equipped  
        
        //Public properties for encapsulation (SOLID)
        public string ItemName => itemName;
        public string Description => description;
        public Sprite Icon => icon;
        public GameObject ModelPrefab => modelPrefab;
        public int MaxStack => maxStack;
        public int CurrentStack
        {
            get => currentStack;
            set => currentStack = value;
        }
        public ItemType ItemType => itemType;

        //add stats if needed - e.g. damage, healthRestoreAmt, etc.
        
        //Default Implementation - override in derived class
        public virtual void Use(GameObject user)
        {
            Debug.Log($"Used {itemName}");
        }
    }
    
        
}
