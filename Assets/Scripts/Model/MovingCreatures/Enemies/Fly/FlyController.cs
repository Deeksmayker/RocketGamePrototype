using UnityEngine;

public class FlyController : MonoBehaviour
{
    [Header("Jerking")]
    [SerializeField] private float minTimeBeforeJerk, maxTimeBeforeJerk;
    private float _timeToJerk;
    private float _currentTimeAfterJerk;
    [SerializeField] private float minJerkDistance, maxJerkDistance;
    [SerializeField, Range(0f, 1f)] private float jerkSmooth;
    private Vector2 _currentJerkVector;
    private float _currentJerkDistance;
    private Vector2 _positionToMove;

    [Header("Staying")]
    [SerializeField] private float stayMoveDiff;
    [SerializeField, Range(0f, 1f)] float stayJerkingSpeedMultiplier;

    private Rigidbody2D _rb;

    private bool _jerking;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _timeToJerk = Random.Range(minTimeBeforeJerk, maxTimeBeforeJerk);
    }

    private void FixedUpdate()
    {
        if (_jerking)
        {
            Jerk();
            return;
        }

        _currentTimeAfterJerk += Time.deltaTime;

        if (_currentTimeAfterJerk >= _timeToJerk)
        {
            _currentJerkVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            _currentJerkDistance = Random.Range(minJerkDistance, maxJerkDistance);

            var hit = Physics2D.Raycast(transform.position, _currentJerkVector * _currentJerkDistance);
            int counter = 0;
            while (hit != null || counter != 10)
            {
                _currentJerkVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                _currentJerkDistance = Random.Range(minJerkDistance, maxJerkDistance);
                hit = Physics2D.Raycast(transform.position, _currentJerkVector * _currentJerkDistance);
                counter++;
            }
            _positionToMove = _currentJerkVector * _currentJerkDistance;
            _jerking = true;
            return;
        }

        MoveOnStay();
    }

    private void Jerk()
    {
        var newX = Mathf.SmoothDamp(transform.position.x, _positionToMove.x, ref 1, jerkSmooth);
        
    }

    private void MoveOnStay()
    {
        var diffVector = new Vector2(Random.Range(-stayMoveDiff, stayMoveDiff), Random.Range(-stayMoveDiff, stayMoveDiff));
        var newPosition = Vector2.Lerp(transform.position, (Vector2)transform.position + diffVector, stayJerkingSpeedMultiplier);
        _rb.MovePosition(newPosition);
    }
}
