using UnityEngine;
using Code.Component;
using Code.Component.Items;


namespace Code.Weapon.WeaponItem
{
    [CreateAssetMenu(fileName = "Base Dagger", menuName = "Inventory/Weapon/Base Dagger")]
    public class BaseDaggerItem : Item
    {

        public override void Use(GameObject user)
        {
            var interactComponent = user.GetComponent<Code.Component.CharacterInteractComponent>();
            var status = user.GetComponent<CharacterStatusComponent>();
            if (interactComponent == null)
            {
                Debug.LogWarning($"Cannot equip{ItemName}: No CharacterInteractComponent found");
                return;
            }
            GameObject weaponInstance = Instantiate(ModelPrefab);
            IWeapon weapon = weaponInstance.GetComponent<IWeapon>();
            if (weapon == null)
            {
                Debug.LogWarning($"Weapon prefab for {ItemName} missing from IWeapon component.");
                Destroy(weaponInstance);
                return;
            }
            interactComponent.SetEquippingWeapon(weapon);
            status.WeaponType = EquippedWeaponType.BaseDagger;
        }
    }
}
