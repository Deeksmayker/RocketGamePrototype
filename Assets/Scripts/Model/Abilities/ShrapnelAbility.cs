using Player;
using System.Collections;
using UnityEngine;

public class ShrapnelAbility : Ability
{
    private RocketLauncher _rocketLauncher;

    [SerializeField] private int perUseCostIncrease;

    private void Start()
    {
        _rocketLauncher = GetComponent<RocketLauncher>();

        _rocketLauncher.shootPerformed.AddListener(() => _shootPerformed = IsActive);
    }

    private bool _shootPerformed;

    public override IEnumerator CastAbility()
    {
        GemCost += perUseCostIncrease;
        abilityCasted.Invoke();
        _rocketLauncher.MakeNextRocketShrapnel();

        var timer = duration;

        while (timer > 0)
        {
            if (_shootPerformed)
            {
                abilityEnded.Invoke();
                _shootPerformed = false;
                _rocketLauncher.RemoveShrapnelFromNextRocket();
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }

        _rocketLauncher.RemoveShrapnelFromNextRocket();

        abilityEnded.Invoke();
    }

    public override void LoadUpgradedValue()
    {
        Debug.LogError("No word in shrapnel");
    }

    public override void DisableAbility()
    {
        abilityEnded.Invoke();
        _shootPerformed = false;
        _rocketLauncher.RemoveShrapnelFromNextRocket();
    }
}
