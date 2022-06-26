using System;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SpiderAiController : MonoBehaviour
    {
        public float speed;
        
        private Rigidbody2D _rb;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (_rb.velocity.x < speed)
                _rb.velocity = new Vector2(speed, 0);
        }
    }
}