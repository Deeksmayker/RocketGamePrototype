using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealerObject : MonoBehaviour
{
    [SerializeField] private float moveAmplitude;
    [SerializeField] private float moveSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerHealth>(out var health))
        {
            health.Heal();
            Destroy(gameObject);
        }
    }
}
