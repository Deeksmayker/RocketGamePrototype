using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Material matBlink;
    [SerializeField] private Material matDefault;

    [Serializable]
    public struct Platforms
    {
        public int time;
        public GameObject map;
        public TilemapRenderer renderer;
    }

    public Platforms[] platforms;
    public float GameTime { get; private set; }

    private int _currentNumOfPlatform;
    private float _startTime;
    bool _isCoroutineStarted;
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
        Debug.Log(_finishTime - _startTime);
        if (_finishTime - GameTime <= 5 && !_isCoroutineStarted)
        {
            _isCoroutineStarted = true;
            StartCoroutine(MakePlatformBlick());
            StopCoroutine(MakePlatformBlick());
        }
        else if (_finishTime - GameTime <= 0)
        {
            ChangePlatforms();
            SetStartAndFinishTimeForPlatform();
            _isCoroutineStarted = false;
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
        platforms[_currentNumOfPlatform].renderer.material = matBlink;
    }

    public void MakePlatformDefault()
    {
        platforms[_currentNumOfPlatform].renderer.material = matDefault;
    }

    public IEnumerator MakePlatformBlick()
    {
        for (int i = 0; i < 5; i++)
        {
            MakePlatformDarker();
            yield return new WaitForSeconds(0.5f);
            MakePlatformDefault();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ChangePlatforms()
    {
        platforms[_currentNumOfPlatform].map.SetActive(false);
        ChangeNumOfCurrentPlatform();
        platforms[_currentNumOfPlatform].map.SetActive(true);
    }
}
