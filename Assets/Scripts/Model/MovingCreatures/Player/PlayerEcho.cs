using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEcho : MonoBehaviour
{
    [SerializeField] private Transform EchoAimTransform;

    public void SetAimRotation(Quaternion rotation)
    {
        EchoAimTransform.rotation = rotation;
    }
}
