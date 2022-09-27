using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text timerText;
    private float _currentTime;

    private void Update()
    {
        Timer();
    }

    public void Timer()
    {
        _currentTime += Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(_currentTime);
        timerText.text = time.ToString(@"s\.fff");
    }
}
