using Assets.Scripts.Model.Interfaces;
using Player;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IGetCaught
{
    [field: SerializeField] public int Health { get; private set; }

    [SerializeField] private float timeForShootAfterGetCaught;

    private bool _getCaught;

    private BouncePlayerController _playerController;
    private RocketLauncher _rocketLauncher;

    [HideInInspector] public UnityEvent GetCaughtEvent = new();
    [HideInInspector] public UnityEvent ReleasedCaughtEvent = new();
    [HideInInspector] public UnityEvent DamagedEvent = new();
    [HideInInspector] public UnityEvent PlayerDiedEvent = new();

    private void Start()
    {
        _playerController = GetComponent<BouncePlayerController>();
        _rocketLauncher = GetComponent<RocketLauncher>();
    }


    public void GetCaught()
    {
        _getCaught = true;
        GetCaughtEvent.Invoke();

        if (_playerController != null)
        {
            _playerController.SetCaught(true);
        }

        if (_rocketLauncher != null)
        {
            Invoke(nameof(DisableShooting), timeForShootAfterGetCaught);
        }
    }

    public void ReleaseCaught()
    {
        _getCaught = false;
        ReleasedCaughtEvent.Invoke();

        if (_playerController != null)
        {
            _playerController.SetCaught(false);
        }

        if (_rocketLauncher != null)
        {
            _rocketLauncher.SetCaught(false);
        }
    }

    public void TakeDamageOnRelease()
    {
        Health--;
        DamagedEvent.Invoke();

        if (Health <= 0)
        {
            PlayerDiedEvent.Invoke();
            Destroy(gameObject);
        }
    }

    private void DisableShooting()
    {
        if (!_getCaught)
            return;
        _rocketLauncher.SetCaught(true);
    }
}
