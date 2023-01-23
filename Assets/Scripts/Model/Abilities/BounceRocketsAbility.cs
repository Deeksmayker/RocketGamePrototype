using Player;
using System.Collections;
using UnityEngine;

public class BounceRocketsAbility : Ability
{
    private RocketLauncher _rocketLauncher;

    [SerializeField] private int perUseCostIncrease;
    [SerializeField] private ParticleSystem particles;

    private bool _shootPerformed;

    private void Start()
    {
        _rocketLauncher = GetComponent<RocketLauncher>();
        
        _rocketLauncher.shootPerformed.AddListener(() => _shootPerformed = IsActive);
    }

    public override IEnumerator CastAbility()
    {
        GemCost += perUseCostIncrease;
        abilityCasted.Invoke();
        particles.Play();
        _rocketLauncher.SetRocketToBounce();
        
        var timer = duration;

        while (timer > 0)
        {
            if (_shootPerformed)
            {
                DisableAbility();
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }

        DisableAbility();
    }

    public override void DisableAbility()
    {
        _shootPerformed = false;
        _rocketLauncher.SetRocketToDefault();
        particles.Stop();
        abilityEnded.Invoke();
    }
}
