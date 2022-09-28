using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text timerText;
    private GameManager _gameManager;
    private TimeSpan time;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
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
}
