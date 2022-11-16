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

    private Rigidbody2D _rb;

    private Vector2 _vectorToPlayer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        RocketLauncher.GlobalShootPreformed.AddListener(RepulseGem);
    }

    private void FixedUpdate()
    {
        _vectorToPlayer = (PlayerController.PlayerPosition - (Vector2)transform.position);

        _rb.velocity = Vector2.Lerp(_rb.velocity, _vectorToPlayer.normalized * maxSpeed, toPlayerAcceleration * Time.fixedDeltaTime);
    }

    private void RepulseGem()
    {
        Debug.Log(1);
        _rb.velocity = -_vectorToPlayer.normalized * repulsionMultiplier * repulsionMultiplier / _vectorToPlayer.magnitude;
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
