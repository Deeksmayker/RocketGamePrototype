using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private TextMeshProUGUI gemCountText;

    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject diedPanel;

    private GameManager _gameManager;
    private PlayerAbilities _playerAbilities;
    private PlayerHealth _playerHealth;
    private TimeSpan time;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _playerAbilities = FindObjectOfType<PlayerAbilities>();
        _playerHealth = FindObjectOfType<PlayerHealth>();

        _playerAbilities.gemTaken.AddListener(() => OnGemTaken());

        _playerHealth.DamagedEvent.AddListener(OnPlayerDamaged);
        _playerHealth.PlayerDiedEvent.AddListener(() => diedPanel.SetActive(true));
    }

    private void Update()
    {
        ChangeTimerText();
    }

    public void ChangeTimerText()
    {
        timerText.text = _gameManager.GameTime.ToString("F3");
        return;

        time = TimeSpan.FromSeconds(_gameManager.GameTime);
        timerText.text = time.ToString(@"s\.fff");
    }

    public void OnGemTaken()
    {
        gemCountText.text = _playerAbilities.GemCount.ToString();
    }

    public void OnPlayerDamaged()
    {
        healthText.text = _playerHealth.Health.ToString();
    }
}
