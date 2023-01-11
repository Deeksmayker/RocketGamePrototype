using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recordText;
    [SerializeField] private TextMeshProUGUI playText, exitText, resetText;

    private void Start()
    {
        UpdateLanguage();
        
        Localization.LanguageChanged.AddListener(() => UpdateLanguage());
    }

    public void OnPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void OnExit()
    {
        Application.Quit();
    }
    
    public void ResetRecord()
    {
        PlayerPrefs.DeleteAll();
        Start();
    }

    public void UpdateLanguage()
    {
        var record = PlayerPrefs.HasKey("Record") ? PlayerPrefs.GetFloat("Record") : 0;
        var languageText = LanguageInfo.Language == LanguageInfo.Languages.Russian ? "Ваш рекорд" : "Your Record";
        recordText.text = $"{languageText} - {record}";
        
        playText.text = LanguageInfo.Language == LanguageInfo.Languages.Russian ? "Играть" : "Play";
        exitText.text = LanguageInfo.Language == LanguageInfo.Languages.Russian ? "Выйти" : "Exit";
        resetText.text = LanguageInfo.Language == LanguageInfo.Languages.Russian ? "Сбросить рекорд" : "Reset record";
    }
}
