using UnityEngine;

namespace Player
{
    public class CollisionDetector : MonoBehaviour
    {
        [Header("Layers")]
        public LayerMask groundLayer;

        [Space]

        public bool onGround;
        public bool onWall;
        public bool onRightWall;
        public bool onLeftWall;
        public int wallSide;

        [Space]

        [Header("Collision")]

        public float collisionRadius = 0.25f;
        public Vector2 bottomOffset, rightOffset, leftOffset;
        protected Color debugCollisionColor = Color.red;

        // Start is called before the first frame update
        protected virtual void Start()
        {
        
        }

        // Update is called once per frame
        protected virtual void Update()
        {  
            onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);

            onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
            onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

            onWall = onRightWall || onLeftWall;

            wallSide = onRightWall ? -1 : 1;
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

            Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        }
    }
}