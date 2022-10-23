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

    private float _platformCreationTime;
    private bool _isPlatformBlinking;
    private float _platformRemovalTime;
    private Material _defaultMaterial;

    private void Start()
    {
        if (_timeChangingObjects.Count == 0)
            return;

        _defaultMaterial = _timeChangingObjects[0].tilemapsToAppear[0].GetComponent<Material>();
        SetPlatformLifetime();
    }

    private void Update()
    {
        if (_timeChangingObjects.Count == 0)
            return;

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
        _platformRemovalTime = _platformCreationTime + _timeChangingObjects[0].timeToAppear;
    }

    public void RemoveCurrentPlatform()
    {
        if (_timeChangingObjects[0].tilemapsToAppear.Count > 0)
        {
            _timeChangingObjects[0].tilemapsToAppear.RemoveAt(0);
        }
    }

    public void ChangeMaterial(Material material)
    {
        _timeChangingObjects[0].tilemapsToAppear[0].GetComponent<TilemapRenderer>().material = material;
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
        if (_timeChangingObjects[0].tilemapsToAppear.Count > 0)
        {
            Instantiate(_timeChangingObjects[0].tilemapsToAppear[0]);
        }
    }
}