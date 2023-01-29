using System;
using Assets.Scripts.Model;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recordText;
    [SerializeField] private TextMeshProUGUI playText, exitText, resetText;
    [SerializeField] private TextMeshProUGUI coinValueText;

    private void Start()
    {
        Time.timeScale = 1;
        
        if (YandexGame.SDKEnabled)
        {
            UpdateLanguage();
        }
        
        Localization.LanguageChanged.AddListener(() => UpdateLanguage());
        
        UpdateCoinValue();
        
    }

    private void OnEnable()
    {
        YandexGame.GetDataEvent += UpdateLanguage;
        SavesManager.OnCoinValueChanged.AddListener(UpdateCoinValue);
    }

    private void OnDisable()
    {
        YandexGame.GetDataEvent -= UpdateLanguage;
        SavesManager.OnCoinValueChanged.RemoveListener(UpdateCoinValue);

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

    public void UpdateCoinValue()
    {
        coinValueText.text = SavesManager.Coins.ToString();
    }

    public void UpdateLanguage()
    {
        var record = YandexGame.savesData.record;
        var languageText = LanguageInfo.Language == LanguageInfo.Languages.Russian ? "Ваш рекорд" : "Your Record";
        recordText.text = $"{languageText} - {record}";
        
        playText.text = LanguageInfo.Language == LanguageInfo.Languages.Russian ? "Начать" : "Play";
        exitText.text = LanguageInfo.Language == LanguageInfo.Languages.Russian ? "Выйти" : "Exit";
        resetText.text = LanguageInfo.Language == LanguageInfo.Languages.Russian ? "Сбросить рекорд" : "Reset record";
    }
}
