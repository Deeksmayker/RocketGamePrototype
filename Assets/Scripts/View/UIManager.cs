using Player;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private TextMeshProUGUI gemCountText;

    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject diedPanel;
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private TextMeshProUGUI newRecordText;
    [SerializeField] private AudioSource newRecordSound;

    [Header("Abilities")]
    [SerializeField] private Button firstAbilityButton;
    [SerializeField] private Button secondAbilityButton;
    [SerializeField] private Button thirdAbilityButton;

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
        PlayerHealth.PlayerDiedEvent.AddListener(() => diedPanel.SetActive(true));

        _gameManager.NewRecordEvent.AddListener(OnNewRecord);

        OnGemTaken();
    }

    private void Update()
    {
        ChangeTimerText();

        if (_input.pause)
        {
            if (pausePanel.activeSelf)
            {
                pausePanel.SetActive(false);
            }
            else
            {
                OpenPausePanel();
            }
            _input.pause = false;
        }
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
        Debug.Log("New record ui");
        newRecordText.gameObject.SetActive(true);
        newRecordSound.Play();
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void UpdateLanguage()
    {
        
    }
}
