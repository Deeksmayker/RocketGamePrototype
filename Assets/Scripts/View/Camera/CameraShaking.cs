using System;
using System.Collections;
using Cinemachine;
using Player;
using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    public static CameraShaking Instance;
    
    [SerializeField] private int shakingForce = 2;
    [SerializeField] private float shakingTime = 0.2f;

    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _basicMultiChannelPerlin;

    private void Awake()
    {
        Instance = this;
        
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _basicMultiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnEnable()
    {
        Rocket.OnRocketExplosion.AddListener(OnRocketExplosion);
    }
    

    private void OnDisable()
    {
        Rocket.OnRocketExplosion.RemoveListener(OnRocketExplosion);
    }

    private void OnRocketExplosion()
    {
        StartCoroutine(StartShake());
    }

    public void SetShake(bool needShake)
    {
        if (needShake)
        {
            _basicMultiChannelPerlin.m_AmplitudeGain = shakingForce;
            _basicMultiChannelPerlin.m_FrequencyGain = shakingForce;
            return;
        }
        
        _basicMultiChannelPerlin.m_AmplitudeGain = 0;
        _basicMultiChannelPerlin.m_FrequencyGain = 0;
    }
    
    private IEnumerator StartShake()
    {
        _basicMultiChannelPerlin.m_AmplitudeGain = shakingForce;
        _basicMultiChannelPerlin.m_FrequencyGain = shakingForce;
        yield return new WaitForSeconds(shakingTime);
        _basicMultiChannelPerlin.m_AmplitudeGain = 0;
        _basicMultiChannelPerlin.m_FrequencyGain = 0;
    }
}