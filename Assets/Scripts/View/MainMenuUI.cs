using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recordText;
    [SerializeField] private TextMeshProUGUI playText, exitText, resetText;

    private void Start()
    {
        if (YandexGame.SDKEnabled)
        {
            UpdateLanguage();
        }
        
        Localization.LanguageChanged.AddListener(() => UpdateLanguage());
    }

    private void OnEnable()
    {
        YandexGame.GetDataEvent += UpdateLanguage;
    }

    private void OnDisable()
    {
        YandexGame.GetDataEvent -= UpdateLanguage;
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
        var record = YandexGame.savesData.record;
        var languageText = LanguageInfo.Language == LanguageInfo.Languages.Russian ? "Ваш рекорд" : "Your Record";
        recordText.text = $"{languageText} - {record}";
        
        playText.text = LanguageInfo.Language == LanguageInfo.Languages.Russian ? "Играть" : "Play";
        exitText.text = LanguageInfo.Language == LanguageInfo.Languages.Russian ? "Выйти" : "Exit";
        resetText.text = LanguageInfo.Language == LanguageInfo.Languages.Russian ? "Сбросить рекорд" : "Reset record";
    }
}
