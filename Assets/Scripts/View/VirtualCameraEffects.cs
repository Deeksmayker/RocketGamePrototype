using Assets.Scripts.Model;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VirtualCameraEffects : MonoBehaviour
{
    [SerializeField] private float camSizeOnPlayerGetCaught;
    [SerializeField] private float camSizeOnRain;
    [SerializeField] private float zoomSpeed;

    [SerializeField] private Volume volume;
    private Vignette _vignette;
    private float _startVignetteIntencity;

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

        _playerHealth.GetCaughtEvent.AddListener(() =>
        {
            _getCaught = true;
            _vignette.color.Override(Color.red);
        });
        _playerHealth.ReleasedCaughtEvent.AddListener(() =>
        {
            _getCaught = false;
            _vignette.color.Override(Color.black);
        });

        volume.profile.TryGet(out _vignette);
        _startVignetteIntencity = _vignette.intensity.GetValue<float>();
    }

    private void Update()
    {
        if (_getCaught)
        {
            _vCam.m_Lens.OrthographicSize = Mathf.Lerp(_vCam.m_Lens.OrthographicSize, camSizeOnPlayerGetCaught, Mathf.Sqrt(zoomSpeed * Time.deltaTime));

            _vignette.intensity.Override(Mathf.Lerp(_vignette.intensity.GetValue<float>(), 1, Mathf.Sqrt(zoomSpeed * Time.deltaTime)));
        }

        else if (!Utils.CompareNumsApproximately(_vCam.m_Lens.OrthographicSize, _currentCamSizeToReturn, 0.01f))
        {
            _vCam.m_Lens.OrthographicSize = Mathf.Lerp(_vCam.m_Lens.OrthographicSize, _currentCamSizeToReturn, Mathf.Sqrt(zoomSpeed * Time.deltaTime));

            _vignette.intensity.Override(Mathf.Lerp(_vignette.intensity.GetValue<float>(), _startVignetteIntencity, Mathf.Sqrt(zoomSpeed * Time.deltaTime)));
        }
    }
}
