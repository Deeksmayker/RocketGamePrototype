using UnityEngine;
using UnityEngine.Pool;

public class GemDropOnDeath : MonoBehaviour
{
    [SerializeField] private Gem gemPrefab;

    private void OnDisable()
    {
        if (Gem.GemPool is default(ObjectPool<Gem>))
        {
            Gem.GemPool = new ObjectPool<Gem>
                (
                    () => Instantiate(gemPrefab),
                    (gem) => gem.gameObject.SetActive(true),
                    (gem) => gem.gameObject.SetActive(false),
                    (gem) => Destroy(gem.gameObject),
                    true,
                    30,
                    50
                );
        }

        var gem = Gem.GemPool.Get();
        gem.transform.position = transform.position;
        gem.Rb.velocity = Vector2.up * 10;
    }
}
