using System.Collections;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider
{
    public class SpiderProceduralAnimationRef : MonoBehaviour
    {
        public Transform[] legTargets; // то, куда нога должна переместиться.
        public float stepSize = 0.15f; // длина шага
        public int smoothness = 8; // кол-во шагав за один цикл ходьбы
        public float stepHeight = 0.15f; // высота подъема ног
        public bool bodyOrientation = true; // бесполезно

        public float raycastRange = 1.5f; 
        private Vector2[] defaultLegPositions; // положение ног по дефолту
        private Vector2[] lastLegPositions; // храним прошлые позиции ног
        private Vector2 lastBodyUp; // последняя позиция по Y
        private bool[] legMoving; // храним какие ноги двигаются
        private int countLegs; // кол-во ног
    
        private Vector2 velocity; // ускорение
        private Vector2 lastVelocity; // прошлое ускорение
        private Vector2 lastBodyPos; // прошлое положение тела

        private float velocityMultiplier = 7f;
        private bool isBlocked = false;

        public void BlockProceduralAnimation()
        {
            isBlocked = true;
        }
        
        public void UnBlockProceduralAnimation()
        {
            isBlocked = false;
        }
        
        public void SetDefaultLegPosition()
        {
            for (int i = 0; i < 4; i++)
            {
                StartCoroutine(PerformStep(i, transform.TransformPoint(defaultLegPositions[i])));
            }
        }
        
        Vector2[] MatchToSurfaceFromAbove(Vector2 point, float halfRange, Vector2 up)
        {
            Vector2[] res = new Vector2[2];
            res[1] = Vector3.zero;
            RaycastHit2D hit;
            hit = Physics2D.Raycast(point , -up, 2f * halfRange);

            Debug.DrawRay(point , - up * 2f * halfRange, Color.red, smoothness * Time.deltaTime);

            if (!hit.collider)
                res[0] = GetClosestPointOnCollider((point));
            else
                res[0] = hit.point;
            
            res[1] = hit.normal;

            return res;
        }
        
        private Vector2 GetClosestPointOnCollider(Vector2 currentLegTransform)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(currentLegTransform, raycastRange); 
            float closestSqrDistance = Mathf.Infinity;
            Vector2 closestPoint = Vector2.zero;
        
            foreach (var col in colliders)
            {
                Vector3 pos = col.ClosestPoint(currentLegTransform);
                float sqrDist = (pos - transform.position).magnitude;

                if (sqrDist < closestSqrDistance)
                {
                    closestSqrDistance = sqrDist;
                    closestPoint = pos;
                }
            }

            var dist = Vector2.Distance(closestPoint, currentLegTransform);
            return dist < 0.3f ? currentLegTransform : closestPoint;
        }

    
        void Start()
        {
            lastBodyUp = transform.up;
            
            countLegs = legTargets.Length;
            defaultLegPositions = new Vector2[countLegs];
            lastLegPositions = new Vector2[countLegs];
            legMoving = new bool[countLegs];
            for (int i = 0; i < countLegs; ++i)
            {
                defaultLegPositions[i] = legTargets[i].localPosition;
                lastLegPositions[i] = legTargets[i].position;
                legMoving[i] = false;
            }
            lastBodyPos = transform.position;
        }

        IEnumerator PerformStep(int index, Vector3 targetPoint)
        {
            Vector2 startPos = lastLegPositions[index];
            for(int i = 1; i <= smoothness; ++i)
            {
                legTargets[index].position = Vector2.Lerp(startPos, targetPoint, i / (float)(smoothness + 1f));
                legTargets[index].position += transform.up * Mathf.Sin(i / (float)(smoothness + 1f) * Mathf.PI) * stepHeight;
                yield return new WaitForFixedUpdate();
            }
            legTargets[index].position = targetPoint;
            lastLegPositions[index] = legTargets[index].position;
            legMoving[0] = false;
        }
        
        void FixedUpdate()
        {
            if (isBlocked)
                return;
            
            velocity = (Vector2)transform.position - lastBodyPos;
            velocity = (velocity + smoothness * lastVelocity) / (smoothness + 1f);

            if (velocity.magnitude < 0.000025f)
                velocity = lastVelocity;
            else
                lastVelocity = velocity;
            
            Vector2[] desiredPositions = new Vector2[countLegs];
            int indexToMove = -1;
            float maxDistance = stepSize;
            for (int i = 0; i < countLegs; ++i)
            {
                desiredPositions[i] = transform.TransformPoint(defaultLegPositions[i]);

                float distance = Vector3.ProjectOnPlane(desiredPositions[i] + velocity * velocityMultiplier - lastLegPositions[i], transform.up).magnitude;
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexToMove = i;
                }
            }
            for (int i = 0; i < countLegs; ++i)
                if (i != indexToMove)
                    legTargets[i].position = lastLegPositions[i];

            if (indexToMove != -1 && !legMoving[0])
            {
                Vector2 targetPoint = desiredPositions[indexToMove];

                Vector2[] positionAndNormalFwd = MatchToSurfaceFromAbove(targetPoint + velocity * velocityMultiplier, raycastRange, 
                    ((Vector2)transform.parent.up - velocity * 10).normalized);

                Vector2[] positionAndNormalBwd = MatchToSurfaceFromAbove(targetPoint + velocity * velocityMultiplier, raycastRange*(1f + velocity.magnitude), 
                    ((Vector2)transform.parent.up + velocity * 10).normalized);
            
                legMoving[0] = true;
            
                
                if (positionAndNormalFwd[1] == Vector2.zero)
                {
                    StartCoroutine(PerformStep(indexToMove, positionAndNormalBwd[0]));
                }
                else
                {
                    StartCoroutine(PerformStep(indexToMove, positionAndNormalFwd[0]));
                }
            }

            lastBodyPos = transform.position;
            if (countLegs > 1 && bodyOrientation)
            {
                Vector2 v1 = (legTargets[1].position - legTargets[0].position).normalized;
            
                Vector2 v2 = Vector3.back;
                Vector2 normal = Vector3.Cross(v1, v2).normalized;
                Vector2 up = Vector3.Lerp(lastBodyUp, normal, 1f / (float)(smoothness + 1));
                transform.up = up;
                transform.rotation = Quaternion.LookRotation(transform.parent.forward, up);
                lastBodyUp = transform.up;
            }
        }

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < countLegs; ++i)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.TransformPoint(defaultLegPositions[i]), 0.2f);
            }
        }
    }
}