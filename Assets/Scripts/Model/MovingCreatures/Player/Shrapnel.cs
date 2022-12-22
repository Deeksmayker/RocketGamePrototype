using Assets.Scripts.Model;
using UnityEngine;

public class Shrapnel : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    private bool _canKill = true;

    [HideInInspector] public Rigidbody2D Rb;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_canKill && collision.gameObject.TryGetComponent<IReactToExplosion>(out var target))
        {
            target.TakeDamage();
        }

        _canKill = false;
    }
}
