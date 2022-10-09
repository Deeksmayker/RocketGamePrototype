using Player;
using UnityEngine;

namespace Assets.Scripts.Model.MovingCreatures.Enemies
{
    public class SpiderCollisionDetector : CollisionDetector
    {
        public bool onCeiling;
        public bool contactingWithGround;

        public Vector2 upOffset;

        protected override void Start()
        {
            base.Start();

        }

        protected override void Update()
        {
            base.Update();
            
            onCeiling = Physics2D.OverlapCircle((Vector2)transform.position + upOffset, collisionRadius, groundLayer);

            contactingWithGround = onGround || onWall || onCeiling;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.DrawWireSphere((Vector2)transform.position + upOffset, collisionRadius);
        }
    }
}
