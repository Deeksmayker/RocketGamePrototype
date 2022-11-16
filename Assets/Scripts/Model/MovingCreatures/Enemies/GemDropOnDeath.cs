using UnityEngine;

public class GemDropOnDeath : MonoBehaviour
{
    [SerializeField] private Gem gemPrefab;

    private void OnDestroy()
    {
        var gem = Instantiate(gemPrefab, transform.position, Quaternion.identity);
        gem.Rb.velocity = Vector2.up * 10;
    }
}
