using UnityEngine;

public class CoinSound : MonoBehaviour
{
    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _audio.Play();
        Invoke(nameof(ReturnToPool), 0.3f);
    }

    private void ReturnToPool()
    {
        Coin.CoinSoundPool.Release(this);
    }
}
