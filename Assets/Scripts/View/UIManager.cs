using System;
using Player;
using System.Collections;
using Assets.Scripts.Model;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private TextMeshProUGUI gemCountText;

    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject diedPanel;
    
    [SerializeField] private Button reviveButton;
    [SerializeField] private Button doubleCoinButton;
    [SerializeField] private Slider doubleCoinSlider;
    
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private TextMeshProUGUI newRecordText;
    [SerializeField] private AudioSource newRecordSound;

    [Header("Abilities")]
    [SerializeField] private Button firstAbilityButton;
    [SerializeField] private Button secondAbilityButton;
    [SerializeField] private Button thirdAbilityButton;

    [Header("Upgrades And Coins")]
    [SerializeField] private TextMeshProUGUI coinCounterText;
    [SerializeField] private TextMeshProUGUI additionalCoinCounterText;
    
    private int _coinCountOnStart;
    private int _earnedCoins;

    private bool _needToSetCoinCounters;

    private GameManager _gameManager;
    private PlayerAbilities _playerAbilities;
    private PlayerHealth _playerHealth;
    private GameInputManager _input;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _playerAbilities = FindObjectOfType<PlayerAbilities>();
        _playerHealth = FindObjectOfType<PlayerHealth>();
        _input = FindObjectOfType<GameInputManager>();

        _playerAbilities.gemTaken.AddListener(() => OnGemTaken());
        _playerAbilities.anyAbilityUsed.AddListener(() => OnGemTaken());

        _playerHealth.DamagedEvent.AddListener(OnPlayerDamaged);
        _playerHealth.HealedEvent.AddListener(OnPlayerHealed);
        
        GameManager.NewRecordEvent.AddListener(OnNewRecord);

        _coinCountOnStart = SavesManager.Coins;
        //Debug.Log(_coinCountOnStart);

        OnGemTaken();

        _gameManager.enabled = true;
    }

    private void OnEnable()
    {
        GameManager.PlayerRevived.AddListener(OnPlayerRevive);
        PlayerHealth.PlayerDiedEvent.AddListener(OnPlayerDied);
        SavesManager.OnCoinValueChanged.AddListener(() => _earnedCoins++);
        YandexGame.RewardVideoEvent += HandleReward;
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= HandleReward;
    }

    private void Update()
    {
        ChangeTimerText();
        
        if (_input.pause)
        {
            if (pausePanel.activeSelf)
            {
                pausePanel.SetActive(false);
                RocketLauncher.InMenu = false;
            }
            else
            {
                OpenPausePanel();
                RocketLauncher.InMenu = true;
            }
            _input.pause = false;
        }
    }

    public void OpenRewardAd(int id)
    {
        YandexGame.RewVideoShow(id);
    }

    public void HandleReward(int id)
    {
        if (id == 1)
        {
            _gameManager.RevivePlayer();
        }

        if (id == 2)
        {
            SavesManager.Coins += _earnedCoins;
            YandexGame.savesData.coins = SavesManager.Coins;
            YandexGame.SaveProgress();

            _earnedCoins *= 2;
            additionalCoinCounterText.text = "+" + _earnedCoins.ToString();
            _needToSetCoinCounters = true;
        }
    }

    public void OnPlayerRevive()
    {
        OnPlayerDamaged();
        diedPanel.SetActive(false);

        _coinCountOnStart = SavesManager.Coins;
    }

    public void OnPlayerDied()
    {
        diedPanel.SetActive(true);

        coinCounterText.text = _coinCountOnStart.ToString();
        additionalCoinCounterText.text = "+" + _earnedCoins.ToString();
        
        if (GameManager.DiedCount <= 1)
            reviveButton.gameObject.SetActive(true);
        else
        {
            reviveButton.gameObject.SetActive(false);
            doubleCoinButton.gameObject.SetActive(true);
            StartCoroutine(SetCoinCounters());
        }

        if (YandexGame.savesData.record < GameManager.GameTime)
        {
            YandexGame.savesData.record = GameManager.GameTime;
            YandexGame.savesData.coins = SavesManager.Coins;
            //YandexGame.SaveCloud();
            //YandexGame.SaveLocal();
            GameManager.NewRecordEvent.Invoke();
            YandexGame.NewLBScoreTimeConvert("Record", GameManager.GameTime);

            if (GameManager.GameTime > 150)
            {
                YandexGame.ReviewShow(true);
            }
        }

        YandexGame.savesData.coins = SavesManager.Coins;
        YandexGame.SaveProgress();
    }

    public void OpenPausePanel()
    {
        pausePanel.SetActive(true);
    }

    public void ChangeTimerText()
    {
        timerText.text = GameManager.GameTime.ToString("F3");
    }

    public void OnGemTaken()
    {
        gemCountText.text = _playerAbilities.GemCount.ToString();
        UpdateAbilityButtons();
    }

    public void UpdateAbilityButtons()
    {
        SetAbilityInteraction(_playerAbilities.FirstAbility, firstAbilityButton);
        SetAbilityInteraction(_playerAbilities.SecondAbility, secondAbilityButton);
        SetAbilityInteraction(_playerAbilities.ThirdAbility, thirdAbilityButton);

        firstAbilityButton.GetComponentInChildren<TextMeshProUGUI>().text = "Z \n" + _playerAbilities.FirstAbility.GemCost.ToString();
        secondAbilityButton.GetComponentInChildren<TextMeshProUGUI>().text = "X \n" + _playerAbilities.SecondAbility.GemCost.ToString();
        thirdAbilityButton.GetComponentInChildren<TextMeshProUGUI>().text = "C \n" + _playerAbilities.ThirdAbility.GemCost.ToString();
        
        GameManager.PlayerRevived.AddListener(() => OnPlayerDamaged());
    }

    public void SetAbilityInteraction(Ability ability, Button abilityButton)
    {
        if (_playerAbilities.GemCount < ability.GemCost)
        {
            abilityButton.interactable = false;
        }
        else
        {
            abilityButton.interactable = true;
        }
    }

    public void OnPlayerDamaged()
    {
        healthText.text = _playerHealth.Health.ToString();
    }

    public void OnPlayerHealed()
    {
        healthText.text = _playerHealth.Health.ToString();
    }

    public void OnNewRecord()
    {
        //Debug.Log("New record ui");
        newRecordText.gameObject.SetActive(true);
        newRecordSound.Play();
    }

    public void ToMainMenu()
    {
        YandexGame.FullscreenShow();
        SceneManager.LoadScene(0);
    }

    public void ReloadLevel()
    {
        if (GameManager.GameTime > 150)
            YandexGame.FullscreenShow();
        SceneManager.LoadScene(1);
    }

    public void UpdateLanguage()
    {
        
    }

    [SerializeField] private Slider soundSlider;

    public void ManageSound()
    {
        AudioListener.volume = soundSlider.value;
    }

    private IEnumerator SetCoinCounters()
    {
        var timer = 1f;

        while (timer > 0 && !_needToSetCoinCounters)
        {
            timer -= Time.deltaTime / 5;
            doubleCoinSlider.value = timer;
            yield return null;
        }
        
        doubleCoinButton.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(1);

        while (_earnedCoins > 0)
        {
            _earnedCoins--;
            _coinCountOnStart++;
            Coin.CoinSoundPool.Get();
            
            coinCounterText.text = _coinCountOnStart.ToString();
            additionalCoinCounterText.text = "+" + _earnedCoins.ToString();
            yield return new WaitForSeconds(0.005f);
        }

        additionalCoinCounterText.text = "";
    }
}
