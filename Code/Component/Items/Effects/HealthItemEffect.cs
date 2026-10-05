using UnityEngine;

namespace Code.Component.Items.Effects
{
    [CreateAssetMenu(fileName = "New Health Potion", menuName = "Inventory/Items/Health Potion")]
    public class HealthItemEffect : Item
    {
        [SerializeField] private float healAmount = 20f;

        public override void Use(GameObject user)
        {
            // Try to use IHealthReceiver interface first (works with CharacterHealthComponent)
            var healthReceiver = user.GetComponent<IHealthReceiver>();
            if (healthReceiver != null)
            {
                healthReceiver.AddHealth(healAmount);
                return;
            }

            // Fallback to CharacterStatusComponent for backward compatibility
            var status = user.GetComponent<CharacterStatusComponent>();
            if (status != null) status.AddHealth(healAmount);
        }
    }
}
