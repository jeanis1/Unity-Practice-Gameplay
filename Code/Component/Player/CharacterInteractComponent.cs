using UnityEngine;
using Code.Weapon;
using UnityEngine.Serialization;

namespace Code.Component
{
    public interface IInteract
    {
        // bool AttemptEquipWeapon();
        void EnableWeaponHit(float damage);
        void DisableWeaponHit();
        IWeapon GetEquippedWeapon();

        bool OverlappedWeaponExists();
        void SetEquippingWeapon(IWeapon weapon); // inventory equipping
        void RemoveEquippedWeapon();
        void SetOverlappedInteractable(GameObject obj);  //for interactTarget
        void RemoveOverlappedInteractable(GameObject obj);
        bool HasOverlappedInteractable();
        GameObject GetOverlappedInteractable();
        
    }
    public class CharacterInteractComponent : MonoBehaviour, IInteract
    {
        [FormerlySerializedAs("weaponAttachPoint")]
        [SerializeField] private Transform weaponRightAttachPoint;
        [SerializeField] private Transform weaponLeftAttachPoint;
        private IWeapon equippedWeapon;// actual equipped weapon
        private GameObject overlappedInteractable ; //to set interactTarget in InteractState
        
        public void EnableWeaponHit(float damage)
        {
            if (equippedWeapon == null) return;
            equippedWeapon.damageAmt = damage; // damage must be set first before enable Damage.
            equippedWeapon.damageEnabled = true;
        }

        public void DisableWeaponHit()
        {
            if (equippedWeapon == null) return;
            equippedWeapon.damageEnabled = false;
            equippedWeapon.damageAmt = 0f;
        }

        public IWeapon GetEquippedWeapon()
        {
            return equippedWeapon;
        }

        public bool OverlappedWeaponExists()
        {
            return overlappedInteractable != null && overlappedInteractable.GetComponent<IWeapon>() != null;
        }
        
        public void SetEquippingWeapon(IWeapon weapon)
        {
            if (equippedWeapon != null)
            {
                MonoBehaviour weaponMono = equippedWeapon as MonoBehaviour;
                if (weaponMono != null)
                {
                    equippedWeapon.Unequip();
                    Destroy(weaponMono.gameObject);  // clean up old weapon instance
                }
            }
            equippedWeapon = weapon;
            if (weapon != null)
            {
                weapon.Equip(weaponRightAttachPoint, weaponLeftAttachPoint, gameObject);
            }
        }

        public void RemoveEquippedWeapon()
        {
            if (equippedWeapon == null) return;
            equippedWeapon.Unequip();
            equippedWeapon.SetActive(false); //for now simply hide inactive. if large number of weapons hidden need to change to remove from memory.
        }
        public void SetOverlappedInteractable(GameObject obj)
        {
            overlappedInteractable = obj; //Last overlapped wins
        }

        public void RemoveOverlappedInteractable(GameObject obj)
        {
            if (overlappedInteractable == obj)
                overlappedInteractable = null;
        }

        public bool HasOverlappedInteractable()
        {
            return overlappedInteractable != null;
        }

        public GameObject GetOverlappedInteractable()
        {
            return overlappedInteractable;
        }
        

    }
}