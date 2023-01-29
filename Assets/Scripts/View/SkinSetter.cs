using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

public class SkinSetter : MonoBehaviour
{
    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();

        if (SavesManager.Skin == null)
            return;
        
        if (SavesManager.Skin.name != "Square")
            _sr.color = Color.white;
        else
            _sr.color = Color.yellow;
        
        _sr.sprite = SavesManager.Skin;
    }
}
