using Assets.Scripts.Model.Interfaces;
using Player;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class Gem : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float toPlayerAcceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float repulsionMultiplier;

    [HideInInspector] public Rigidbody2D Rb;

    public static ObjectPool<Gem> GemPool;

    private Vector2 _vectorToPlayer;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();

        RocketLauncher.GlobalShootPreformed.AddListener(RepulseGem);

        if (GemPool is default(ObjectPool<Gem>))
        {
            GemPool = new ObjectPool<Gem>
                (
                    () => Instantiate(this),
                    (gem) => gem.gameObject.SetActive(true),
                    (gem) => gem.gameObject.SetActive(false),
                    (gem) => Destroy(gem.gameObject),
                    true,
                    30,
                    50
                ) ;
        }
    }

    private void FixedUpdate()
    {
        _vectorToPlayer = (BouncePlayerController.PlayerPosition - (Vector2)transform.position);
        if (_vectorToPlayer.x is float.NaN)
            return;
        Rb.velocity = Vector2.Lerp(Rb.velocity, _vectorToPlayer.normalized * maxSpeed, toPlayerAcceleration * Time.fixedDeltaTime);
    }

    private void RepulseGem()
    {
        if (_vectorToPlayer.x is float.NaN || Rb.velocity.x is float.NaN)
            return;
        Rb.velocity = repulsionMultiplier * repulsionMultiplier * -_vectorToPlayer.normalized / _vectorToPlayer.magnitude;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var possibleGemTaker = collision.gameObject
            .GetComponents<MonoBehaviour>()
            .OfType<ITakeGem>()
            .FirstOrDefault();

        if (possibleGemTaker == default(ITakeGem))
            return;


        possibleGemTaker.TakeGem();
        GemPool.Release(this);
    }
}
