using Assets.Scripts.Model;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BouncePlayerController))]
public class SlowTimeAbility : Ability
{
    [SerializeField] private float slowTimeScale;
    [SerializeField] private float slowingTimeDuration;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private int perUseCostIncrease = 2;

    private BouncePlayerController _playerController;

    private void Start()
    {
        _playerController = GetComponent<BouncePlayerController>();
    }

    public override IEnumerator CastAbility()
    {
        GemCost += perUseCostIncrease;
        abilityCasted.Invoke();

        var lerpMultiplier = 0f;
        while (!Utils.CompareNumsApproximately(_playerController.GetWalkSpeed(), _playerController.OriginalWalkSpeed * speedMultiplier, 0.01f))
        {
            lerpMultiplier += Time.unscaledDeltaTime / slowingTimeDuration;
            _playerController.SetWalkSpeed(
                Mathf.Lerp(_playerController.OriginalWalkSpeed, _playerController.OriginalWalkSpeed * speedMultiplier, lerpMultiplier)
                );
            Time.timeScale = Mathf.Lerp(1, slowTimeScale, lerpMultiplier);
            yield return null;
        }

        var timer = duration;
        while (timer > 0)
        {
            Time.timeScale = slowTimeScale;
            timer -= Time.unscaledDeltaTime;
            yield return null;
        }

        lerpMultiplier = 0f;
        while (!Utils.CompareNumsApproximately(_playerController.OriginalWalkSpeed, _playerController.GetWalkSpeed(), 0.001f))
        {
            lerpMultiplier += Time.unscaledDeltaTime / slowingTimeDuration;
            _playerController.SetWalkSpeed(
                Mathf.Lerp(_playerController.OriginalWalkSpeed * speedMultiplier, _playerController.OriginalWalkSpeed, lerpMultiplier)
                );
            Time.timeScale = Mathf.Lerp(slowTimeScale, 1, lerpMultiplier);
            yield return null;
        }

        abilityEnded.Invoke();
    }

    public override void LoadUpgradedValue()
    {
        duration = SavesManager.GetSlowTimeDuration();
    }

    public override void DisableAbility()
    {
        Time.timeScale = 1;
    }
}
