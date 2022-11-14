using Assets.Scripts.Model.Interfaces;
using System.Linq;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float toPlayerAcceleration;
    [SerializeField] private float repulsionMultiplier;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var vectorToPlayer = (PlayerController.PlayerPosition - (Vector2)transform.position).normalized;

        _rb.AddForce(vectorToPlayer * toPlayerAcceleration, ForceMode2D.Impulse);
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
