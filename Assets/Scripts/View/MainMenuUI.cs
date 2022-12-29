using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recordText;

    private void Start()
    {
        var record = PlayerPrefs.HasKey("Record") ? PlayerPrefs.GetFloat("Record") : 0;
        recordText.text = $"Your Record - {record}";
    }

    public void OnPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
