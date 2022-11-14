using Assets.Scripts.Model.Interfaces;
using Player;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GameInputManager))]
public class PlayerAbilities : MonoBehaviour, ITakeGem
{
    [field: SerializeField] public int GemCount { get; private set; }

    private Ability _firstAbility, _secondAbility, _thirdAbility;

    private GameInputManager _input;

    private bool _firstAbilityActive, _secondAbilityActive, _thirdAbilityActive;

    public UnityEvent firstAbilityCasted = new();
    public UnityEvent firstAbilityEnded = new();


    private void Start()
    {
        _input = GetComponent<GameInputManager>();
        _firstAbility = GetComponent<SlowTimeAbility>();
        _secondAbility = GetComponent<ShrapnelAbility>();

        _firstAbility.abilityEnded.AddListener(() =>
        {
            firstAbilityEnded.Invoke();
            _firstAbilityActive = false;
        });
    }

    private void Update()
    {
        if (_input.firstAbility && !_firstAbilityActive)
        {
            firstAbilityCasted.Invoke();
            StartCoroutine(_firstAbility.CastAbility());
        }

        _input.firstAbility = false;
        _input.secondAbility = false;
        _input.thirdAbility = false;
    }

    public void TakeGem()
    {
        GemCount++;
    }
}
