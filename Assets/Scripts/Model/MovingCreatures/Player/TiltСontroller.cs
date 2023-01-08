using UnityEngine;
using static BouncePlayerController.AirStates;

namespace Model.MovingCreatures.Player
{
    public class TiltСontroller : MonoBehaviour
    {
   
        [SerializeField] private float TiltSpeed = 0.1f;
        
        private Rigidbody2D rigidbody;
        private BouncePlayerController bouncePlayerController;

        private void Start()
        {
            rigidbody = GetComponentInParent<Rigidbody2D>();
            bouncePlayerController = GetComponentInParent<BouncePlayerController>();
        }

        private void Update()
        {
            if (bouncePlayerController.AirState is Grounded or Bouncing)
                transform.rotation = Quaternion.Euler(0, 0, -rigidbody.velocity.x);
            else
                transform.Rotate(new Vector3(0,0, -rigidbody.velocity.x * TiltSpeed * Time.deltaTime));
        }
    }
}
