using Code.Component.Items.Pools;
using UnityEngine;


namespace Code.Component.Items.Pickups
{
    [RequireComponent(typeof(Collider))]
    public class CoinPickup : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            //TODO - award player gold here....

            CoinPool.Instance.ReturnToPool(this);

        }
    }
}