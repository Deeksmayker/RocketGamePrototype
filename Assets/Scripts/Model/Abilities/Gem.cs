using Assets.Scripts.Model.Interfaces;
using Player;
using System.Linq;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float toPlayerAcceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float repulsionMultiplier;

    [HideInInspector] public Rigidbody2D Rb;

    private Vector2 _vectorToPlayer;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();

        RocketLauncher.GlobalShootPreformed.AddListener(RepulseGem);
    }

    private void FixedUpdate()
    {
        _vectorToPlayer = (BouncePlayerController.PlayerPosition - (Vector2)transform.position);

        Rb.velocity = Vector2.Lerp(Rb.velocity, _vectorToPlayer.normalized * maxSpeed, toPlayerAcceleration * Time.fixedDeltaTime);
    }

    private void RepulseGem()
    {
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
        Destroy(gameObject);
    }
}
