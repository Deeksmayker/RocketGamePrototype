using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class CoinDropper : MonoBehaviour
{
    [SerializeField] private Coin coinPrefab;
    [SerializeField] private ParticleSystem particles;
    
    [SerializeField] private int coinCount;
    
    public static ObjectPool<Coin> CoinPool;

    private void Awake()
    {
        if (CoinPool == null)
        {
            CoinPool = new ObjectPool<Coin>(
                () => Instantiate(coinPrefab),
                coin => coin.gameObject.SetActive(true),
                coin => coin.gameObject.SetActive(false),
                coin => Destroy(coin.gameObject),
                true,
                100,
                300);
        }
    }

    private void OnDisable()
    {
        Instantiate(particles, transform.position, Quaternion.identity);
        for (var i = 0; i < coinCount; i++)
        {
            var coin = CoinPool.Get();
            coin.transform.position = transform.position;
            coin.rb.velocity = new Vector2(Random.Range(-1, 1), 1) * Random.Range(10, 30);
            coin.SetupCoin();
        }
    }
}
