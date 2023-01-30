using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class SkillUpgrades : MonoBehaviour
{
    [SerializeField] private Button slowTimeUpgradeButton;
    [SerializeField] private Button bounceUpgradeButton;
    [SerializeField] private Button radiusUpgradeButton;
    [SerializeField] private Button startTimeUpgradeButton;

    [SerializeField] private TextMeshProUGUI slowTimeCostText;
    [SerializeField] private TextMeshProUGUI slowTimeDurationText;
    [SerializeField] private TextMeshProUGUI bounceCostText;
    [SerializeField] private TextMeshProUGUI bounceDurationText;
    [SerializeField] private TextMeshProUGUI radiusCostText;
    [SerializeField] private TextMeshProUGUI radiusValueText;
    [SerializeField] private TextMeshProUGUI startTimeCostText;
    [SerializeField] private TextMeshProUGUI startTimeValueText;

    [SerializeField] private TextMeshProUGUI coinText;

    private void Awake()
    {
        SavesManager.SlowTimeUpgradeLevel = YandexGame.savesData.SlowTimeUpgradeLevel;
        SavesManager.BounceUpgradeLevel = YandexGame.savesData.BounceUpgradeLevel;
        SavesManager.RadiusUpgradeLevel = YandexGame.savesData.RadiusUpgradeLevel;
        SavesManager.StartTimeUpgradeLevel = YandexGame.savesData.StartTimeUpgradeLevel;
    }
    
    private void OnEnable()
    {
        LoadCostsAndValues();
        UpdateInteractableForButtons();
        YandexGame.SaveProgress();

        coinText.text = YandexGame.savesData.coins.ToString();
    }

    public void LoadCostsAndValues()
    {
        slowTimeCostText.text = SavesManager.GetUpgradeCost(SavesManager.SlowTimeUpgradeLevel).ToString();
        bounceCostText.text = SavesManager.GetUpgradeCost(SavesManager.BounceUpgradeLevel).ToString();
        radiusCostText.text = SavesManager.GetUpgradeCost(SavesManager.RadiusUpgradeLevel).ToString();
        startTimeCostText.text = SavesManager.GetUpgradeCost(SavesManager.StartTimeUpgradeLevel).ToString();

        slowTimeDurationText.text = SavesManager.GetSlowTimeDuration() + "";
        bounceDurationText.text = SavesManager.GetBounceDuration() + "";
        radiusValueText.text = SavesManager.GetRadiusValue() + "";
        startTimeValueText.text = SavesManager.GetStartTimeValue() + "";
    }

    public void UpdateInteractableForButtons()
    {
        CheckTextAndSetInteractable(slowTimeUpgradeButton, slowTimeCostText, SavesManager.SlowTimeUpgradeLevel);
        CheckTextAndSetInteractable(bounceUpgradeButton, bounceCostText, SavesManager.BounceUpgradeLevel);
        CheckTextAndSetInteractable(radiusUpgradeButton, radiusCostText, SavesManager.RadiusUpgradeLevel);
        CheckTextAndSetInteractable(startTimeUpgradeButton, startTimeCostText, SavesManager.StartTimeUpgradeLevel, 21);
    }

    public void HandleSlowTimeUpgradeClick()
    {
        SavesManager.Coins -= SavesManager.GetUpgradeCost(SavesManager.SlowTimeUpgradeLevel);
        SavesManager.SlowTimeUpgradeLevel++;
        YandexGame.savesData.coins = SavesManager.Coins;
        YandexGame.savesData.SlowTimeUpgradeLevel = SavesManager.SlowTimeUpgradeLevel;
        OnEnable();
    }
    
    public void HandleBounceUpgradeClick()
    {
        SavesManager.Coins -= SavesManager.GetUpgradeCost(SavesManager.BounceUpgradeLevel);
        SavesManager.BounceUpgradeLevel++;
        YandexGame.savesData.coins = SavesManager.Coins;
        YandexGame.savesData.BounceUpgradeLevel = SavesManager.BounceUpgradeLevel;
        OnEnable();
    }
    
    public void HandleRadiusUpgradeClick()
    {
        SavesManager.Coins -= SavesManager.GetUpgradeCost(SavesManager.RadiusUpgradeLevel);
        SavesManager.RadiusUpgradeLevel++;
        YandexGame.savesData.coins = SavesManager.Coins;
        YandexGame.savesData.RadiusUpgradeLevel = SavesManager.RadiusUpgradeLevel;
        
        OnEnable();
    }

    public void HandleStartTimeUpgrade()
    {
        SavesManager.Coins -= SavesManager.GetUpgradeCost(SavesManager.StartTimeUpgradeLevel);
        SavesManager.StartTimeUpgradeLevel++;
        YandexGame.savesData.coins = SavesManager.Coins;
        YandexGame.savesData.StartTimeUpgradeLevel = SavesManager.StartTimeUpgradeLevel;
        OnEnable();
    }

    private void CheckTextAndSetInteractable(Button button, TextMeshProUGUI costText, int level, int maxLevel = 15)
    {
        if (int.Parse(costText.text) > SavesManager.Coins || level >= maxLevel)
        {
            button.interactable = false;

            if (level >= maxLevel)
            {
                costText.text = "MAX";
            }
        }
        else
        {
            button.interactable = true;
        }
    }
}
