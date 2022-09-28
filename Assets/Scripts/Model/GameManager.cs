using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Material matBlink;
    [SerializeField] private Material matDefault;

    [Serializable]
    public struct Platforms
    {
        public int time;
        public GameObject map;
    }
    public Platforms[] platforms;
    public float GameTime { get; private set; }

    private int _currentNumOfPlatform;
    private float _startTime;
    private float _finishTime;

    private void Start()
    {
        matDefault = platforms[_currentNumOfPlatform].map.GetComponent<Material>();
        _currentNumOfPlatform = 0;
        SetStartAndFinishTimeForPlatform();
    }

    private void Update()
    {
        UpdateGameTime();

        if (_finishTime - _startTime <= 5)
        {
            MakePlatformBlick();
        }
        else if (_finishTime - _startTime <= 0)
        {
            ChangePlatforms();
            SetStartAndFinishTimeForPlatform();
        }
    }

    public void UpdateGameTime()
    {
        GameTime += Time.deltaTime;
    }

    public void SetStartAndFinishTimeForPlatform()
    {
        _startTime = GameTime;
        _finishTime = _startTime + platforms[_currentNumOfPlatform].time;
    }

    public void ChangeNumOfCurrentPlatform()
    {
        _currentNumOfPlatform++;
    }

    public void MakePlatformDarker()
    {
        //platforms[_currentNumOfPlatform].map.GetComponent<Material>() = matBlink;
    }

    public void MakePlatformDefault()
    {
        //platforms[_currentNumOfPlatform].map = matDefault;
    }

    public void MakePlatformBlick()
    {
        if (_finishTime - _startTime / 2 == 0)
        {
            MakePlatformDarker();
        }
        else
        {
            MakePlatformDefault();
        }
    }

    public void ChangePlatforms()
    {
        platforms[_currentNumOfPlatform].map.SetActive(false);
        ChangeNumOfCurrentPlatform();
        //platforms[_currentNumOfPlatform].map.SetActive(true);
    }
}
