using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using Player;
using UnityEngine;

public class ExplosionRadiusAbility : Ability
{
    private RocketLauncher _rocketLauncher;

    [SerializeField] private float explosionRadiusMultiplier;
    [SerializeField] private int perUseCostIncrease;
    [SerializeField] private ParticleSystem particles;

    private void Start()
    {
        _rocketLauncher = GetComponent<RocketLauncher>();
    }

    public override IEnumerator CastAbility()
    {
        GemCost += perUseCostIncrease;
        abilityCasted.Invoke();
        particles.Play();
        _rocketLauncher.SetRadiusMultiplier(explosionRadiusMultiplier);
        yield return new WaitForSeconds(duration);
        _rocketLauncher.SetRadiusMultiplier(1);
        
        abilityEnded.Invoke();
        particles.Stop();
    }

    public override void LoadUpgradedValue()
    {
        explosionRadiusMultiplier = SavesManager.GetRadiusValue();
    }

    public override void DisableAbility()
    {
        _rocketLauncher.SetRadiusMultiplier(1);
        abilityEnded.Invoke();
        particles.Stop();
    }
}
