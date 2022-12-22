using Assets.Scripts.Model;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BouncePlayerController))]
public class SlowTimeAbility : Ability
{
    [SerializeField] private float slowTimeScale;
    [SerializeField] private float slowingTimeDuration;
    [SerializeField] private float speedMultiplier;

    private BouncePlayerController _playerController;

    private void Start()
    {
        _playerController = GetComponent<BouncePlayerController>();
    }

    public override IEnumerator CastAbility()
    {
        abilityCasted.Invoke();

        var lerpMultiplier = 0f;
        while (!Utils.CompareNumsApproximately(slowTimeScale, Time.timeScale, 0.01f))
        {
            lerpMultiplier += Time.unscaledDeltaTime / slowingTimeDuration;
            _playerController.SetWalkSpeed(
                Mathf.Lerp(_playerController.OriginalWalkSpeed, _playerController.OriginalWalkSpeed * speedMultiplier, lerpMultiplier)
                );
            Time.timeScale = Mathf.Lerp(1, slowTimeScale, lerpMultiplier);
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        lerpMultiplier = 0f;
        while (!Utils.CompareNumsApproximately(1, Time.timeScale, 0.001f))
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
}
