using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCollider : MonoBehaviour
{
    [SerializeField] private Transform teleportPos;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        col.transform.position = teleportPos.position;
    }
}
