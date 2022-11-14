using Assets.Scripts.Model;
using System.Collections;
using UnityEngine;

public class SlowTimeAbility : Ability
{
    [SerializeField] private float slowTimeScale;
    [SerializeField] private float slowingTimeDuration;
    [SerializeField] private float speedMultiplier;

    public override IEnumerator CastAbility()
    {
        var lerpMultiplier = 0f;
        while (!Utils.CompareNumsApproximately(slowTimeScale, Time.timeScale, 0.01f))
        {
            lerpMultiplier += Time.unscaledDeltaTime / slowingTimeDuration;
            playerController.SetWalkSpeed(
                Mathf.Lerp(playerController.OriginalWalkSpeed, playerController.OriginalWalkSpeed * speedMultiplier, lerpMultiplier)
                );
            Time.timeScale = Mathf.Lerp(1, slowTimeScale, lerpMultiplier);
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        lerpMultiplier = 0f;
        while (!Utils.CompareNumsApproximately(1, Time.timeScale, 0.001f))
        {
            lerpMultiplier += Time.unscaledDeltaTime / slowingTimeDuration;
            playerController.SetWalkSpeed(
                Mathf.Lerp(playerController.OriginalWalkSpeed * speedMultiplier, playerController.OriginalWalkSpeed, lerpMultiplier)
                );
            Time.timeScale = Mathf.Lerp(slowTimeScale, 1, lerpMultiplier);
            yield return null;
        }

        abilityEnded.Invoke();
    }
}
