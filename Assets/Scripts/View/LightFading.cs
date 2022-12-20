using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightFading : MonoBehaviour
{
    [SerializeField] private float timeBeforeFading;
    [SerializeField] private float fadingDuration;

    private Light2D _light;

    private float _timePassed;
    private float t;
    private float startIntensity;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        t = 0f;
        startIntensity = _light.intensity;
    }

    private void Update()
    {
        if (_timePassed < timeBeforeFading)
        {
            _timePassed += Time.deltaTime;
            return;
        }

        t += Time.deltaTime / fadingDuration;
        _light.intensity = Mathf.Lerp(startIntensity, 0, t);
    }
}
