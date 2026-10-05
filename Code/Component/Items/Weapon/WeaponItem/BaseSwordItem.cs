using Code.Component;
using UnityEngine;
using Code.Component.Items;

namespace Code.Weapon.WeaponItem
{
    [CreateAssetMenu(fileName = "Base Sword", menuName = "Inventory/Weapon/Base Sword")]
    public class BaseSwordItem : Item
    {
        public override void Use(GameObject user)
        {
            var interactComponent = user.GetComponent<Code.Component.CharacterInteractComponent>();
            var status = user.GetComponent<CharacterStatusComponent>();
            if (interactComponent == null)
            {
                Debug.LogWarning($"Cannot equip {ItemName}: No CharacterInteractComponent found");
                return;
            }
            
            //Instantiate weapon from prefab
            GameObject weaponInstance = Instantiate(ModelPrefab);
            IWeapon weapon = weaponInstance.GetComponent<IWeapon>();

            if (weapon == null)
            {
                Debug.LogWarning($"Weapon prefab for {ItemName} missing IWeapon component");
                Destroy(weaponInstance);
                return;
            }
            interactComponent.SetEquippingWeapon(weapon); //attach to player called here
            status.WeaponType = EquippedWeaponType.BaseSword;
        }
    }
}
