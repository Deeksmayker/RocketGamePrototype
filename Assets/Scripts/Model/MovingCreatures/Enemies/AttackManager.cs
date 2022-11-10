using Assets.Scripts.Model.Interfaces;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AttackManager : MonoBehaviour, ICapture
{
    [SerializeField] private LayerMask layersToCheck;

    [SerializeField] private Transform attackCheckStartPoint;
    [SerializeField] private float attackCheckRadius;

    [SerializeField] private Transform captureTargetPoint;

    [SerializeField] private float timeToKill;

    [SerializeField] private int checksPerSecond = 20;
    private float _tickRate;
    private float _tickTimer;

    private bool _capture;

    private IStopMoving _stopMovingScript;

    public UnityEvent enemyKilled = new();
    public UnityEvent enemyCaptured = new();

    private void Start()
    {
        _tickRate = 1f / checksPerSecond;
        _tickTimer = _tickRate;

        _stopMovingScript = GetComponent<IStopMoving>();

        if (_stopMovingScript == default(IStopMoving))
        {
            Destroy(this);
            Debug.LogError("Interface for stop moving is not implemented on this object, attack manager script was deleted");
        }

        enemyKilled.AddListener(() => _capturedTarget.ReleaseCaught());
    }

    private void Update()
    {
        if (_capture)
            return;

        _tickTimer -= Time.deltaTime;
        if (_tickTimer <= 0)
        {
            CheckAttack();
            _tickTimer = _tickRate;
        }
    }

    private IGetCaught _capturedTarget;
    private void CheckAttack()
    {
        var possibleTarget = Physics2D.OverlapCircle(attackCheckStartPoint.position, attackCheckRadius, layersToCheck);

        if (possibleTarget == null)
            return;

        _capturedTarget = possibleTarget.GetComponents<MonoBehaviour>().OfType<IGetCaught>().FirstOrDefault();

        if (_capturedTarget == default(IGetCaught))
            return;

        _capturedTarget.GetCaught();
        StartCoroutine(Capture(possibleTarget.gameObject));
    }

    public IEnumerator Capture(GameObject target)
    {
        enemyCaptured.Invoke();
        _capture = true;

        var timer = timeToKill;
        _stopMovingScript.StopMoving();

        while (timer > 0)
        {
            if (target == null)
                yield break;

            timer -= Time.deltaTime;
            target.transform.position = Vector2.Lerp(target.transform.position, captureTargetPoint.position, Time.deltaTime * 10);
            yield return null;
        }

        enemyKilled.Invoke();
        yield return new WaitForSeconds(0.5f);
        _capture = false;
        _stopMovingScript.ResumeMoving();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackCheckStartPoint.position, attackCheckRadius);
    }
}
