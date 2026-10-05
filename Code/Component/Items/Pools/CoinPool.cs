using System.Collections.Generic;
using Code.Component.Items.Pickups;
using UnityEngine;

namespace Code.Component.Items.Pools
{
    public class CoinPool : MonoBehaviour
    {
        public static CoinPool Instance { get; private set; }
        [SerializeField] private CoinPickup coinPrefab;
        [SerializeField] private int poolSize = 20;

        private Queue<CoinPickup> _available = new();

        private void Awake()
        {
            Instance = this;

            for (int i = 0; i < poolSize; i++)
            {
                var coin = Instantiate(coinPrefab, transform);
                coin.gameObject.SetActive(false);
                _available.Enqueue(coin);
            }
        }

        public void SpawnCoin(Vector3 position)
        {
            if (_available.Count == 0)
            {
                Debug.LogWarning("CoinPool exhausted - increase pool size.");
                return;
            }

            var coin = _available.Dequeue();
            coin.transform.position = position;
            coin.gameObject.SetActive(true);
        }

        public void ReturnToPool(CoinPickup coin)
        {
            coin.gameObject.SetActive(false);
            _available.Enqueue(coin);
        }

        public int GetPoolSize()
        {
            return _available.Count;
        }
    }
}

