using UnityEngine;

namespace Code.Component
{
    public class CharacterStateComponent : MonoBehaviour
    {
        public CharacterActionStatus CurrentAction { get; set; } = CharacterActionStatus.NoAction;
        public EquippedWeaponType WeaponType { get; set; } = EquippedWeaponType.NoWeapon;
    }
}
