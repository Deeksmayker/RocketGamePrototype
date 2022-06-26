using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider
{
    public class SpiderProceduralAnimation : MonoBehaviour
    {
        public Transform[] legTargets;
        public float stepLenght;
        public float stepHeight;
        public int smoothness;
        public float rayCastRange;
        public int maxLegsMovingSimultaneouslyCount;

        private int _currentMovingLegsCount;
        private Vector2[] _currentLegPositions;
        private float[] _nextLegTargetDegree;
        private bool[] _legsInMove;
        private Queue<Transform> _legsMoveQueue;

        private int _legsCount;

        private void Start()
        {
            _legsCount = legTargets.Length;
            _nextLegTargetDegree = new float[_legsCount];
            _currentLegPositions = new Vector2[_legsCount];
            _legsInMove = new bool[_legsCount];
            _legsMoveQueue = new Queue<Transform>();

            for (var i = 0; i < _legsCount; i++)
            {
                _currentLegPositions[i] = legTargets[i].position;
                _legsInMove[i] = false;
                _nextLegTargetDegree[i] = GetDegreeBetweenVectors(transform.localPosition, legTargets[i].localPosition);
            }
        }
        
        private void Update()
        {
            for (var i = 0; i < _legsCount; i++)
            {
                var hit = GetRayHit(i);

                if (hit.collider == null)
                    return;

                if (Vector2.Distance(hit.point, _currentLegPositions[i]) > stepLenght && !_legsInMove[i] && !_legsMoveQueue.Contains(legTargets[i]))
                {
                    _legsMoveQueue.Enqueue(legTargets[i]);
                }
            }
            if (!IsMaxLegsMovingSimultaneously() && _legsMoveQueue.Count() != 0)
                TakeLegFromQueueAndPerformStep();
            KeepUnmovingLegsStay();
        }

        private IEnumerator PerformStep(int index, Vector3 targetPoint)
        {
            _legsInMove[index] = true;
            _currentMovingLegsCount += 1;
            Vector2 startPos = _currentLegPositions[index];
            for(int i = 1; i <= smoothness; ++i)
            {
                legTargets[index].position = Vector2.Lerp(startPos, targetPoint, i / (float)(smoothness + 1f));
                legTargets[index].position += transform.up * Mathf.Sin(i / (float)(smoothness + 1f) * Mathf.PI) * stepHeight;
                yield return new WaitForFixedUpdate();
            }
            legTargets[index].position = targetPoint;
            _currentLegPositions[index] = legTargets[index].position;
            _legsInMove[index] = false;
            _currentMovingLegsCount -= 1;
        }

        private void TakeLegFromQueueAndPerformStep()
        {
            var legToMove = _legsMoveQueue.Dequeue();
            var legIndex = legTargets.TakeWhile(leg => leg != legToMove).Count();
            var hit = GetRayHit(legIndex);

            StartCoroutine(PerformStep(legIndex, hit.point));
        }

        private void KeepUnmovingLegsStay()
        {
            for (var i = 0; i < _legsCount; i++)
            {
                if (!_legsInMove[i])
                {
                    legTargets[i].position = _currentLegPositions[i];
                }
            }
        }

        private RaycastHit2D GetRayHit(int index)
        {
            var rayDirection = GetVectorFromDegree(_nextLegTargetDegree[index]).normalized;
            var hit = Physics2D.Raycast(transform.position, rayDirection, rayCastRange);
            
            //Debug.DrawRay(transform.position, rayDirection * rayCastRange, Color.green, 3f);
            return hit;
        }

        private float GetDegreeBetweenVectors(Vector2 first, Vector2 second) =>
            Mathf.Atan2(second.y - first.y, second.x - first.x) * 180f / Mathf.PI;

        private Vector2 GetVectorFromDegree(float degree) => new Vector2(
            Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));

        private bool IsMaxLegsMovingSimultaneously() => maxLegsMovingSimultaneouslyCount == _currentMovingLegsCount;
    }
}