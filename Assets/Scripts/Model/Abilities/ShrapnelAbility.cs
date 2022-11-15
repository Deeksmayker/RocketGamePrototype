using Player;
using System.Collections;
using UnityEngine;

public class ShrapnelAbility : Ability
{
    private RocketLauncher _rocketLauncher;

    private void Start()
    {
        _rocketLauncher = GetComponent<RocketLauncher>();

        _rocketLauncher.shootPerformed.AddListener(() => _shootPerformed = IsActive);
    }

    private bool _shootPerformed;

    public override IEnumerator CastAbility()
    {
        abilityCasted.Invoke();
        _rocketLauncher.SetRocketToShrapnel();

        var timer = duration;

        while (timer > 0)
        {
            if (_shootPerformed)
            {
                abilityEnded.Invoke();
                _shootPerformed = false;
                _rocketLauncher.SetRocketToDefault();
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }

        _rocketLauncher.SetRocketToDefault();

        abilityEnded.Invoke();
    }
}
