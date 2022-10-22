using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public Transform leftUpArenaCorner, rightDownArenaCorner;

    public List<TimeChangingObjects> timeChangingObjects;
    public float GameTime { get; private set; }

    [Serializable]
    public struct TimeChangingObjects
    {
        public int timeToAppear;
        public List<Tilemap> tilemapsToAppear;
    }

    private float _platformCreationTime;
    private bool _changingTilemaps;
    private float _platformRemovalTime;

    [SerializeField] private Material blinkMaterial;
    private Material _defaultMaterial;

    private void Start()
    {
        if (timeChangingObjects.Count == 0)
            return;
        _defaultMaterial = timeChangingObjects[0].tilemapsToAppear[0].GetComponent<Material>();
        SetPlatformLifetime();
    }

    private void Update()
    {
        UpdateGameTime();

        if (timeChangingObjects.Count == 0)
            return;

        if (_platformRemovalTime - GameTime <= 5 && !_changingTilemaps)
        {
            _changingTilemaps = true;
            StartCoroutine(StartPlatformBlinking());
            StopCoroutine(StartPlatformBlinking());
        }
        else if (_platformRemovalTime - GameTime <= 0)
        {
            ChangePlatforms();
            SetPlatformLifetime();
            _changingTilemaps = false;
        }
    }

    private void UpdateGameTime()
    {
        GameTime += Time.deltaTime;
    }

    public void SetPlatformLifetime()
    {
        _platformCreationTime = GameTime;
        _platformRemovalTime = _platformCreationTime + timeChangingObjects[0].timeToAppear;
    }

    public void RemoveCurrentPlatform()
    {
        timeChangingObjects[0].tilemapsToAppear.RemoveAt(0);
    }

    public void ChangeMaterial(Material material)
    {

        timeChangingObjects[0].tilemapsToAppear[0].GetComponent<TilemapRenderer>().material = material;
    }

    public IEnumerator StartPlatformBlinking()
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
        Instantiate(timeChangingObjects[0].tilemapsToAppear[0]);
    }
}
