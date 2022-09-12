using Assets.Scripts.Model;
using System.Collections;
using UnityEngine;

public class FlyController : MonoBehaviour
{
    [Header("Jerking")]
    [SerializeField] private float minTimeBeforeJerk;
    [SerializeField] private float maxTimeBeforeJerk;
    private float _timeToJerk;
    private float _currentTimeAfterJerk;
    [SerializeField] private float minJerkDistance, maxJerkDistance;
    [SerializeField] private float jerkSpeed;
    private Vector2 _currentJerkVector;
    private float _currentJerkDistance;
    private Vector2 _newPostiion;

    [Header("Staying")]
    [SerializeField] private float stayMoveDiff;
    [SerializeField, Range(0f, 1f)] float stayJerkingSpeedMultiplier;

    private Rigidbody2D _rb;

    private bool _calculatingJerkDirection;
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

        if (_currentTimeAfterJerk >= _timeToJerk && !_calculatingJerkDirection)
        {
            StartCoroutine(CalculateJerkDirection());
            return;
        }

        MoveOnStay();
    }

    private void Jerk()
    {
        var directionWithDistance = _newPostiion - (Vector2)transform.position;
        _rb.MovePosition((Vector2)transform.position + jerkSpeed * Time.deltaTime * directionWithDistance);

        if (Utils.CompareVectors(transform.position, _newPostiion, 1))
        {
            _jerking = false;
            _timeToJerk = Random.Range(minTimeBeforeJerk, maxTimeBeforeJerk);
            _currentTimeAfterJerk = 0;
        }

    }

    private IEnumerator CalculateJerkDirection()
    {
        _calculatingJerkDirection = true;

        _currentJerkVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        _currentJerkDistance = Random.Range(minJerkDistance, maxJerkDistance);

        var hit = Physics2D.Raycast(transform.position, _currentJerkVector, _currentJerkDistance);
        while (hit.distance != 0)
        {
            _currentJerkVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            _currentJerkDistance = Random.Range(minJerkDistance, maxJerkDistance);
            hit = Physics2D.Raycast(transform.position, _currentJerkVector, _currentJerkDistance);
            yield return null;
        }

        _newPostiion = (Vector2)transform.position + _currentJerkVector * _currentJerkDistance;
        _calculatingJerkDirection = false;
        _jerking = true;
    }

    private void MoveOnStay()
    {
        var diffVector = new Vector2(Random.Range(-stayMoveDiff, stayMoveDiff), Random.Range(-stayMoveDiff, stayMoveDiff));
        var newPosition = Vector2.Lerp(transform.position, (Vector2)transform.position + diffVector, stayJerkingSpeedMultiplier);
        _rb.MovePosition(newPosition);
    }
}