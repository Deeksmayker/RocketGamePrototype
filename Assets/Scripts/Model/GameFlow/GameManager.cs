using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public List<TimeChangingObjects> timeChangingObjects;
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
        _defaultMaterial = timeChangingObjects[0].tilemapsToAppear[0].GetComponent<Material>();
        SetPlatformLifetime();
    }

    private void Update()
    {
        UpdateGameTime();
        switch (_platformRemovalTime - GameTime)
        {
            case <= 5 when !_isPlatformBlinking:
                _isPlatformBlinking = true;
                StartCoroutine(MakePlatformBlink());
                StopCoroutine(MakePlatformBlink());
                break;
            case <= 0:
                ChangePlatforms();
                SetPlatformLifetime();
                _isPlatformBlinking = false;
                break;
        }
    }

    private void UpdateGameTime()
    {
        GameTime += Time.deltaTime;
    }

    private void SetPlatformLifetime()
    {
        _platformCreationTime = GameTime;
        _platformRemovalTime = _platformCreationTime + timeChangingObjects[0].timeToAppear;
    }

    private void RemoveCurrentPlatform()
    {
        if (timeChangingObjects[0].tilemapsToAppear.Count > 0)
        {
            timeChangingObjects[0].tilemapsToAppear.RemoveAt(0);
        }
    }

    private void ChangeMaterial(Material material)
    {
        timeChangingObjects[0].tilemapsToAppear[0].GetComponent<TilemapRenderer>().material = material;
    }

    private IEnumerator MakePlatformBlink()
    {
        for (var i = 0; i < 5; i++)
        {
            ChangeMaterial(blinkMaterial);
            yield return new WaitForSeconds(0.5f);
            ChangeMaterial(_defaultMaterial);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void ChangePlatforms()
    {
        RemoveCurrentPlatform();
        CreateNewPlatform();
    }

    private void CreateNewPlatform()
    {
        if (timeChangingObjects[0].tilemapsToAppear.Count > 0)
        {
            Instantiate(timeChangingObjects[0].tilemapsToAppear[0]);
        }
    }
}