using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class RocketLauncher : MonoBehaviour
    {
        [SerializeField] private GameObject rocket;
        [SerializeField] private Transform rocketStartPoint;
        [SerializeField] private GameObject rocketLauncherPivotPoint;

        [SerializeField] private float shootCooldown;
        private float _currentCooldown;
        private bool _canShoot;

        private GameInputManager _input;

        private Vector2 _currentAimDirection;
        
        private void Awake()
        {
            _currentCooldown = shootCooldown;
            _input = GetComponent<GameInputManager>();
        }

        private void Update()
        {
            TurnLauncherOnMouse();

            if (!_canShoot)
            {
                _currentCooldown -= Time.deltaTime;
                if (_currentCooldown <= 0)
                {
                    _canShoot = true;
                    _currentCooldown = shootCooldown;
                }
            }

            if (_input.shoot && _canShoot)
            {
                Shoot();
                _canShoot = false;
            }
            _input.shoot = false;
        }

        private void TurnLauncherOnMouse()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(_input.mousePosition);
            _currentAimDirection = (mousePosition - transform.position).normalized;

            var angle = Mathf.Atan2(_currentAimDirection.y, _currentAimDirection.x) * Mathf.Rad2Deg;
            rocketLauncherPivotPoint.transform.eulerAngles = new Vector3(0, 0, angle);
        }

        private void Shoot()
        {
            var newRocket = Instantiate(rocket, rocketStartPoint.position, Quaternion.identity);
            newRocket.GetComponent<Rocket>().SetDirection(_currentAimDirection);
        }
    }
}