using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float seconds;
    
    private void Awake()
    {
        Destroy(gameObject, seconds);
    }
}
