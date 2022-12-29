using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealerObject : MonoBehaviour
{
    [SerializeField] private AnimationCurve verticalMoving;
    [SerializeField] private float moveCycleTime;

    private Vector2 _startPosition;
    private float _timer;

    private void OnEnable()
    {
        _startPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerHealth>(out var health))
        {
            health.Heal();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        transform.position = new Vector2(_startPosition.x, _startPosition.y + verticalMoving.Evaluate(_timer/moveCycleTime));
        _timer += Time.deltaTime;

        if (_timer > moveCycleTime)
            _timer = 0;
    }
}
