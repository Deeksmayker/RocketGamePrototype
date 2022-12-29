using Assets.Scripts.Model.Interfaces;
using Player;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IGetCaught
{
    [field: SerializeField] public int Health { get; private set; }

    [SerializeField] private float timeForShootAfterGetCaught;
    [SerializeField] private float invinsibilityAfterGetCaught;

    private bool _getCaught;
    private bool _invinsible;
    private bool _canTakeDamage = true;

    private BouncePlayerController _playerController;
    private RocketLauncher _rocketLauncher;

    [HideInInspector] public UnityEvent GetCaughtEvent = new();
    [HideInInspector] public UnityEvent ReleasedCaughtEvent = new();
    [HideInInspector] public UnityEvent DamagedEvent = new();
    public static UnityEvent PlayerDiedEvent = new();
    [HideInInspector] public UnityEvent HealedEvent = new();

    private void Start()
    {
        _playerController = GetComponent<BouncePlayerController>();
        _rocketLauncher = GetComponent<RocketLauncher>();
    }

    public void Heal()
    {
        if (Health >= 3)
            Debug.LogError("Heal object has appear when health is full");

        Health++;
        HealedEvent.Invoke();
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

        _invinsible = true;
        Invoke(nameof(RemoveInvinsibility), invinsibilityAfterGetCaught);
    }

    public void TakeDamageOnRelease()
    {
        if (!_canTakeDamage)
            return;
        Health--;

        _canTakeDamage = false;
        Invoke(nameof(SetCanTakeDamage), 0.5f);

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

    private void SetCanTakeDamage()
    {
        _canTakeDamage = true;
    }

    private void RemoveInvinsibility()
    {
        _invinsible = false;
    }

    public bool CanGetCaught() => !_playerController.GetCaught && !_invinsible;
}
