using UnityEngine;
using UnityEngine.UI;
using System;
using Player;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text timerText;
    private GameManager _gameManager;
    private TimeSpan time;

    [SerializeField] private GameObject leftStick, rightStick;

    private GameInputManager _input;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _input = FindObjectOfType<GameInputManager>();
    }

    private void Update()
    {
        if (timerText != null)
            ChangeTimerText();
    }

    public void ChangeTimerText()
    {
        time = TimeSpan.FromSeconds(_gameManager.GameTime);
        timerText.text = time.ToString(@"s\.fff");
    }
}
