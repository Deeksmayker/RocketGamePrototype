using Player;
using System.Collections;
using UnityEngine;

public class BounceRocketsAbility : Ability
{
    private RocketLauncher _rocketLauncher;

    private void Start()
    {
        _rocketLauncher = GetComponent<RocketLauncher>();
    }

    public override IEnumerator CastAbility()
    {
        abilityCasted.Invoke();
        _rocketLauncher.SetRocketToBounce();
        yield return new WaitForSeconds(duration);
        _rocketLauncher.SetRocketToDefault();
        abilityEnded.Invoke();
    }
}
