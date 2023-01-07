using Assets.Scripts.Model;
using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VirtualCameraEffects : MonoBehaviour
{
    public static VirtualCameraEffects Instance;
    
    [SerializeField] private float camSizeOnPlayerGetCaught;
    [SerializeField] private float zoomSpeed;

    [SerializeField] private Volume volume;
    private Vignette _vignette;
    private float _startVignetteIntencity;

    private float _originalCamSize;
    private float _currentCamSizeToReturn;

    private bool _getCaught;

    private CinemachineVirtualCamera _vCam;
    private PlayerHealth _playerHealth;
    private SlowTimeAbility _slowTimeAbility;

    private void Start()
    {
        Instance = this;
        
        _vCam = GetComponent<CinemachineVirtualCamera>();
        _playerHealth = FindObjectOfType<PlayerHealth>();

        _slowTimeAbility = FindObjectOfType<SlowTimeAbility>();
        _slowTimeAbility.abilityCasted.AddListener(() => SetBlueVignetteStatus(true));
        _slowTimeAbility.abilityEnded.AddListener(() => SetBlueVignetteStatus(false));

        _originalCamSize = _vCam.m_Lens.OrthographicSize;
        _currentCamSizeToReturn = _originalCamSize;
        
        volume.profile.TryGet(out _vignette);
        _startVignetteIntencity = _vignette.intensity.GetValue<float>();

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
        _playerHealth.DamagedEvent.AddListener(() =>
        {
            StartCoroutine(SmoothChangeVignetteIntensity(_startVignetteIntencity, 1 ,0.5f, true));
            StartCoroutine(SmoothChangeVignetteColor(Color.black, Color.red, 0.5f, true));
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

    public void SetBlueVignetteStatus(bool status)
    {
        if (status)
        {
            StartCoroutine(SmoothChangeVignetteColor(Color.black, Color.blue, 0.3f));
            return;
        }

        StartCoroutine(SmoothChangeVignetteColor(Color.blue, Color.black, 0.3f));
    }

    public IEnumerator SmoothChangeVignetteColor(Color startColor, Color needColor, float timeToChange, bool needToReturnColor = false, float timeToReturn = 0)
    {
        var t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime / timeToChange;

            _vignette.color.Override(Color.Lerp(startColor, needColor, t));
            yield return null;
        }

        if (needToReturnColor)
        {
            yield return new WaitForSeconds(timeToReturn);
            StartCoroutine(SmoothChangeVignetteColor(needColor, startColor, timeToChange));
        }
    }
    
    public IEnumerator SmoothChangeVignetteIntensity(float startValue, float endValue, float timeToChange, bool needToReturn)
    {
        var t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime / timeToChange;

            _vignette.intensity.Override(Mathf.Lerp(startValue, endValue, Mathf.Sqrt(t)));
            yield return null;
        }

        if (needToReturn)
        {
            StartCoroutine(SmoothChangeVignetteIntensity(endValue, startValue, timeToChange, false));
        }
    }
}
