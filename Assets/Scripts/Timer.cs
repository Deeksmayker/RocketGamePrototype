using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 

public class Timer : MonoBehaviour
{
    [SerializeField] private Text timerText;
    private float currentTime;

    private void Update()
    {
        currentTime += Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        timerText.text = time.ToString(@"s\.fff");
    }
}
