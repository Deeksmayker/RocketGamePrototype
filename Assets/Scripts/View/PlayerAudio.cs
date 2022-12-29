using Player;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource jumpSound, landingSound, shootSound;

    private BouncePlayerController _player;

    private void OnEnable()
    {
        _player = FindObjectOfType<BouncePlayerController>();

        _player.Jumped.AddListener(PlayJumpSound);
        _player.Bounced.AddListener(PlayJumpSound);
        _player.WallBounced.AddListener(PlayJumpSound);

        _player.Landed.AddListener(PlayLandingSound);

        RocketLauncher.GlobalShootPreformed.AddListener(PlayShootSound);
    }

    private void PlayJumpSound() => jumpSound.Play();
    private void PlayLandingSound() => landingSound.Play();
    private void PlayShootSound() => shootSound.Play();
}
