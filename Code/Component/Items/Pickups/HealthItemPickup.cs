using UnityEngine;

namespace Code.Component.Items.Pickups
{
    public class HealthItemPickup : PickupItem
    {
        [SerializeField] private float healAmount = 20f;

        //Heal Logic here....
        public void UseItem(GameObject player)
        {
            Debug.Log("Heal Logic Called");
        }

    }
}
