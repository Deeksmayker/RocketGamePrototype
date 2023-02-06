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
                    (gem) =>
                    {
                        if (gem == null)
                            gem = Instantiate(gemPrefab);
                        gem.gameObject.SetActive(true);
                    },
                    (gem) => gem.gameObject.SetActive(false),
                    (gem) => Destroy(gem.gameObject),
                    false,
                    30,
                    50
                );
        }

        var gem = Gem.GemPool.Get();
        gem.transform.position = transform.position;
        gem.Rb.velocity = Vector2.up * 10;
    }
}
