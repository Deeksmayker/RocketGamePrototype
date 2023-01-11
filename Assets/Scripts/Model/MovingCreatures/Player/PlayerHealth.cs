using System;
using Assets.Scripts.Model.Interfaces;
using Player;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IGetCaught
{
    [field: SerializeField] public int Health { get; set; }

    [SerializeField] private float timeForShootAfterGetCaught;
    [SerializeField] private float invinsibilityAfterGetCaught;
    [SerializeField] private float immuneToCloudTime = 3;

    private bool _getCaught;
    private bool _invinsible;
    private bool _canTakeDamage = true;
    public bool CanTakeDamageByCloud { get; private set; }= true;

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

    private void OnEnable()
    {
        GameManager.PlayerRevived.AddListener(OnPlayerRevive);
    }

    private void Update()
    {
        if (Health <= 0)
        {
            Die();
        }
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
        CanTakeDamageByCloud = false;
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
        Invoke(nameof(SetCanTakeDamageByCloud), immuneToCloudTime);
    }

    public void TakeDamageOnRelease()
    {
        if (!_canTakeDamage)
            return;
        Health--;

        CanTakeDamageByCloud = false;
        _canTakeDamage = false;
        Invoke(nameof(SetCanTakeDamage), 0.5f);
        Invoke(nameof(SetCanTakeDamageByCloud), immuneToCloudTime);

        DamagedEvent.Invoke();

        if (Health <= 0)
        {
            Die();
        }
    }

    public void TakeDamageByCloud()
    {
        if (!CanTakeDamageByCloud)
            return;

        TakeDamageOnRelease();
    }

    public void OnPlayerRevive()
    {
        CanTakeDamageByCloud = false;
        _canTakeDamage = false;
        
        Invoke(nameof(SetCanTakeDamageByCloud), 3);
        Invoke(nameof(SetCanTakeDamage), 3);
    }

    public void Die()
    {
        GameManager.DiedCount++;
        PlayerDiedEvent.Invoke();
        gameObject.SetActive(false);
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

    private void SetCanTakeDamageByCloud()
    {
        CanTakeDamageByCloud = true;
    }

    private void RemoveInvinsibility()
    {
        _invinsible = false;
    }

    public bool CanGetCaught() => !_playerController.GetCaught && !_invinsible;
}
