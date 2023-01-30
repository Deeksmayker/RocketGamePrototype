using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.Pool;

public class Coin : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    private Vector3 _startVelocity;

    [SerializeField] private CoinSound coinClip;

    [HideInInspector] public Rigidbody2D rb;

    private float _timer;

    public static ObjectPool<CoinSound> CoinSoundPool;

    private void Awake()
    {
        if (CoinSoundPool == null)
        {
            CoinSoundPool = new ObjectPool<CoinSound>(
                () => Instantiate(coinClip),
                clip => clip.gameObject.SetActive(true),
                clip => clip.gameObject.SetActive(false),
                clip => Destroy(clip.gameObject),
                false,
                10,
                20);
        }
        
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _startVelocity = rb.velocity;
        _timer = 0;
    }

    public void SetupCoin()
    {
        _startVelocity = rb.velocity;
        _timer = 0;
    }

    private void Update()
    {
        if (GameManager.TimeSinceStart < 1)
            CoinDropper.CoinPool.Release(this);
        
        transform.Rotate(150 * Time.deltaTime, 150 * Time.deltaTime, 150 * Time.deltaTime);
    }

    private void FixedUpdate()
    {
         _timer += Time.fixedDeltaTime / 1.5f;
        var direction = ((Vector3) BouncePlayerController.PlayerPosition - transform.position).normalized;
        
        rb.velocity = Vector2.Lerp(_startVelocity, direction * maxSpeed, Mathf.Pow(Mathf.Clamp(_timer, 0, 1), 2));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        SavesManager.Coins++;

        CoinSoundPool.Get();
        
        CoinDropper.CoinPool.Release(this);
    }
}
