using UnityEngine;

public class ResolutionController : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(1366, 768, true);
    }
}
