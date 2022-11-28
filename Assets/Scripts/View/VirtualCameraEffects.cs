using Assets.Scripts.Model;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VirtualCameraEffects : MonoBehaviour
{
    [SerializeField] private float camSizeOnPlayerGetCaught;
    [SerializeField] private float camSizeOnRain;
    [SerializeField] private float zoomSpeed;

    private float _originalCamSize;
    private float _currentCamSizeToReturn;

    private bool _getCaught;

    private CinemachineVirtualCamera _vCam;
    private PlayerHealth _playerHealth;

    private void Start()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
        _playerHealth = FindObjectOfType<PlayerHealth>();

        _originalCamSize = _vCam.m_Lens.OrthographicSize;
        _currentCamSizeToReturn = _originalCamSize;

        _playerHealth.GetCaughtEvent.AddListener(() => _getCaught = true);
        _playerHealth.ReleasedCaughtEvent.AddListener(() => _getCaught = false);
    }

    private void Update()
    {
        if (_getCaught)
        {
            _vCam.m_Lens.OrthographicSize = Mathf.Lerp(_vCam.m_Lens.OrthographicSize, camSizeOnPlayerGetCaught, Mathf.Sqrt(zoomSpeed * Time.deltaTime));
        }

        else if (!Utils.CompareNumsApproximately(_vCam.m_Lens.OrthographicSize, _currentCamSizeToReturn, 0.01f))
        {
            _vCam.m_Lens.OrthographicSize = Mathf.Lerp(_vCam.m_Lens.OrthographicSize, _currentCamSizeToReturn, Mathf.Sqrt(zoomSpeed * Time.deltaTime));
        }
    }
}
