using Player;
using UnityEngine;

public class ShrapnelRocket : MonoBehaviour
{
    [SerializeField] private Shrapnel shrapnelPrefab;
    [SerializeField] private float shrapnelCount;
    [SerializeField] private float shrapnelSpeed;

    private void Awake()
    {
        transform.localPosition = Vector2.zero;
    }

    private void OnDestroy()
    {
        for (var i = 0; i < shrapnelCount; i++)
        {
            var shrapnel = Instantiate(shrapnelPrefab, transform.position, Quaternion.identity);
            shrapnel.transform.eulerAngles = new Vector3(0, 0, i * 360 / shrapnelCount);
            shrapnel.Rb.velocity = shrapnel.transform.up * shrapnelSpeed;
        }
    }
}