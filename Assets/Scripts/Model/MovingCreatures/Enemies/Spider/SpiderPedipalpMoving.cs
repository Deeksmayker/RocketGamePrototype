using UnityEngine;

public class SpiderPedipalpMoving : MonoBehaviour
{
    [SerializeField] private Transform[] pedipalpTargets;
    [SerializeField] private Transform capturePoint;

    private Vector3[] defaultPedipalpsPosition;

    private void Start()
    {
        defaultPedipalpsPosition = new Vector3[pedipalpTargets.Length];

        for (int i = 0; i < pedipalpTargets.Length; i++)
            defaultPedipalpsPosition[i] = pedipalpTargets[i].localPosition;
        
        var spiderMoving = transform.parent.GetComponentInParent<SpiderMoving>();
        spiderMoving.OnStartKillFly.AddListener(MovePedipalpsOnCapturePoint);
        spiderMoving.OnStopKillFly.AddListener(MovePedipalpsOnDefault);
    }

    private void MovePedipalpsOnDefault()
    {
        for (int i = 0; i < pedipalpTargets.Length; i++)
            pedipalpTargets[i].localPosition = defaultPedipalpsPosition[i];
    }

    private void MovePedipalpsOnCapturePoint()
    {
        for (int i = 0; i < pedipalpTargets.Length; i++)
            pedipalpTargets[i].position = capturePoint.position;
    }
}