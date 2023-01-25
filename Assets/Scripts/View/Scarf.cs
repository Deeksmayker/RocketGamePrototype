using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarf : MonoBehaviour
{
    public int length;

    [SerializeField] private Transform followTarget;
    [SerializeField] private float distanceBetweenSegments;
    [SerializeField] private float smoothSpeed;
    
    private Vector3[] _segmentPositions;
    private Vector3[] _segmentsVelocity;
    
    private LineRenderer _lr;

    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = length;
        _segmentPositions = new Vector3[length];
        _segmentsVelocity = new Vector3[length];
    }

    private void Update()
    {
        _segmentPositions[0] = followTarget.position;

        for (var i = 1; i < _segmentPositions.Length; i++)
        {
            _segmentPositions[i] = Vector3.SmoothDamp(_segmentPositions[i],
                    _segmentPositions[i - 1] + followTarget.right * distanceBetweenSegments,
                    ref _segmentsVelocity[i],
                    smoothSpeed);
        }

        _lr.SetPositions(_segmentPositions);
    }
}
