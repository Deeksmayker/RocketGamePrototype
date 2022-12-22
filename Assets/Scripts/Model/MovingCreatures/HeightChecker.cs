using UnityEngine;

public class HeightChecker : MonoBehaviour
{
    [SerializeField] private float bottomPosToCheckBelow = -10;
    [SerializeField] private float upPosToTeleport = 80;

    [SerializeField] private float delta = 1;
    [SerializeField] private float maxTimeToBeBelow = 10;

    private float _timer;

    private void Awake()
    {
        maxTimeToBeBelow += Random.Range(-2f, 2f);
        InvokeRepeating(nameof(CheckPos), 0, delta);
    }

    private void CheckPos()
    {
        if (transform.position.y > bottomPosToCheckBelow || Mathf.Abs(transform.position.y - BouncePlayerController.PlayerPosition.y) <= 20)
        {
            _timer = 0;
            return;
        }

        _timer += delta;

        if (_timer > maxTimeToBeBelow)
        {
            transform.position = new Vector2(transform.position.x, upPosToTeleport);
        }
    }
}
