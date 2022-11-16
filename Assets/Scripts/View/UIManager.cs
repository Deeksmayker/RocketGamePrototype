using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private TextMeshProUGUI gemCountText;

    private GameManager _gameManager;
    private PlayerAbilities _playerAbilities;
    private TimeSpan time;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _playerAbilities = FindObjectOfType<PlayerAbilities>();

        _playerAbilities.gemTaken.AddListener(() => OnGemTaken());
    }

    private void Update()
    {
        ChangeTimerText();
    }

    public void ChangeTimerText()
    {
        time = TimeSpan.FromSeconds(_gameManager.GameTime);
        timerText.text = time.ToString(@"s\.fff");
    }

    public void OnGemTaken()
    {
        gemCountText.text = _playerAbilities.GemCount.ToString();
    }
}
