using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public List<TimeChangingObjects> _timeChangingObjects;
    public float GameTime { get; private set; }

    [SerializeField] private Material blinkMaterial;
    [Serializable]
    public struct TimeChangingObjects
    {
        public int timeToAppear;
        public List<Tilemap> tilemapsToAppear;
    }

    private int _currentNumOfPlatform;
    private float _platformCreationTime;
    private bool _isPlatformBlinking;
    private float _platformRemovalTime;
    private Material _defaultMaterial;

    private void Start()
    {
        _defaultMaterial = _timeChangingObjects[_currentNumOfPlatform].tilemapsToAppear[_currentNumOfPlatform].GetComponent<Material>();
        _currentNumOfPlatform = 0;
        SetPlatformLifetime();
    }

    private void Update()
    {
        UpdateGameTime();
        if (_platformRemovalTime - GameTime <= 5 && !_isPlatformBlinking)
        {
            _isPlatformBlinking = true;
            StartCoroutine(MakePlatformBlick());
            StopCoroutine(MakePlatformBlick());
        }
        else if (_platformRemovalTime - GameTime <= 0)
        {
            ChangePlatforms();
            SetPlatformLifetime();
            _isPlatformBlinking = false;
        }
    }

    public void UpdateGameTime()
    {
        GameTime += Time.deltaTime;
    }

    public void SetPlatformLifetime()
    {
        _platformCreationTime = GameTime;
        _platformRemovalTime = _platformCreationTime + _timeChangingObjects[_currentNumOfPlatform].timeToAppear;
    }

    public void RemoveCurrentPlatform()
    {
        _timeChangingObjects[0].tilemapsToAppear.RemoveAt(0);
    }

    public void ChangeMaterial(Material material)
    {

        _timeChangingObjects[_currentNumOfPlatform].tilemapsToAppear[_currentNumOfPlatform].GetComponent<TilemapRenderer>().material = material;
    }

    public IEnumerator MakePlatformBlick()
    {
        for (int i = 0; i < 5; i++)
        {
            ChangeMaterial(blinkMaterial);
            yield return new WaitForSeconds(0.5f);
            ChangeMaterial(_defaultMaterial);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ChangePlatforms()
    {
        RemoveCurrentPlatform();
        CreateNewPlatform();
    }

    public void CreateNewPlatform()
    {
        Instantiate(_timeChangingObjects[0].tilemapsToAppear[0]);
    }
}