using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class testMaterial : MonoBehaviour
{

    private TilemapRenderer renderer;

    private IEnumerator Start()
    {
        renderer = GetComponent<TilemapRenderer>();
        Color materialColor = renderer.material.color;
        
        while (renderer.material.color.a > 0)
        {
            materialColor.a -= 0.5f * Time.deltaTime;
            renderer.material.color = materialColor;
            yield return null;
        }

        yield break;
    }


}
