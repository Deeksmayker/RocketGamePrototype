using System.Collections;
using Cinemachine;
using Player;
using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    [SerializeField] private int shakingForce = 2;
    [SerializeField] private float shakingTime = 0.2f;

    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _basicMultiChannelPerlin;

    private void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _basicMultiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        Rocket.OnRocketExplosion.AddListener(OnRocketExplosion);
    }

    private void OnRocketExplosion()
    {
        StartCoroutine(StartShake());
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