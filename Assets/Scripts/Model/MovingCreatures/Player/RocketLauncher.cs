using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Player
{
    public class RocketLauncher : MonoBehaviour
    {
        public static bool InMenu;
        
        [SerializeField] private Rocket rocket;
        [SerializeField] private BounceHomingRocket bounceRocket;
        [SerializeField] private ShrapnelRocket shrapnelRocketPrefab;
        [SerializeField] private Transform rocketStartPoint;
        [SerializeField] private GameObject rocketLauncherPivotPoint;

        [SerializeField] private float shootCooldown;

        private float _currentCooldown;
        private float _radiusMultiplier = 1;

        private bool _canShoot;
        private bool _isNextRocketShrapnel;
        private bool _getCaught;

        private Vector2 _currentAimDirection;

        private GameInputManager _input;
        private Rocket _currentRocket;

        public static UnityEvent GlobalShootPreformed = new();
        public UnityEvent shootPerformed = new();
        
        private void Awake()
        {
            _currentCooldown = shootCooldown;
            _input = GetComponent<GameInputManager>();
            _currentRocket = rocket;

            InMenu = false;
        }

        private void Update()
        {
            TurnLauncherOnMouse();

            if (_getCaught)
            {
                _input.shoot = false;
                return;
            }

            if (!_canShoot)
            {
                _currentCooldown -= Time.deltaTime;
                if (_currentCooldown <= 0)
                {
                    _canShoot = true;
                    _currentCooldown = shootCooldown;
                }
            }

            if (_input.shoot && _canShoot && !InMenu)
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
            var newRocket = Instantiate(_currentRocket, rocketStartPoint.position, Quaternion.identity);
            newRocket.SetDirection(_currentAimDirection);
            newRocket.explodeRadius *= _radiusMultiplier;

            if (_isNextRocketShrapnel)
            {
                Instantiate(shrapnelRocketPrefab, newRocket.transform);
                _isNextRocketShrapnel = false;
            }

            GlobalShootPreformed.Invoke();
            shootPerformed.Invoke();
        }

        public void SetRocketToBounce()
        {
            _currentRocket = bounceRocket;
        }

        public void SetRocketToDefault()
        {
            _currentRocket = rocket;
        }

        public void SetCaught(bool isGetCaught)
        {
            _getCaught = isGetCaught;
        }

        public void SetRadiusMultiplier(float multiplier)
        {
            _radiusMultiplier = multiplier;
        }

        public void MakeNextRocketShrapnel()
        {
            _isNextRocketShrapnel = true;
        }

        public void RemoveShrapnelFromNextRocket()
        {
            _isNextRocketShrapnel = false;
        }
    }
}