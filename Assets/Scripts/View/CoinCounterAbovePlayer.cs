using Assets.Scripts.Model;
using TMPro;
using UnityEngine;

public class CoinCounterAbovePlayer : MonoBehaviour
{
    private TextMeshProUGUI _coinText;

    private int _currentCount;
    private float _timer;

    private Vector2 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        _coinText = GetComponent<TextMeshProUGUI>();
        _coinText.text = "";
    }

    private void OnEnable()
    {
        SavesManager.OnCoinValueChanged.AddListener(UpdateCoinCount);
    }

    private void OnDisable()
    {
        SavesManager.OnCoinValueChanged.RemoveListener(UpdateCoinCount);
    }

    private void Update()
    {
        MakePulsation();
        transform.position = BouncePlayerController.PlayerPosition + Vector2.up * 2;

        if (_currentCount > 0)
        {
            _timer += Time.deltaTime;

            if (_timer > 3)
            {
                _currentCount = 0;
                _coinText.text = "";
            }
        }
    }

    private void UpdateCoinCount()
    {
        _pulsationSpeed = 20;
        _currentCount++;
        _coinText.text = "+" + _currentCount;
        _timer = 0;
    }

    private float _pulsationSpeed = 5f;
    private void MakePulsation()
    {
        _pulsationSpeed = Mathf.Lerp(_pulsationSpeed, 5, Time.deltaTime * 10);
        
        float scale = Mathf.Sin(Time.time * _pulsationSpeed) / 3;
        
        transform.localScale = new Vector2(transform.localScale.x, originalScale.y + scale);
    }
}
