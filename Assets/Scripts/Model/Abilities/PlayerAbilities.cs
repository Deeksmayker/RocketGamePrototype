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

    public UnityEvent gemTaken = new();
    public UnityEvent anyAbilityUsed = new();

    private void Start()
    {
        _input = GetComponent<GameInputManager>();
        _firstAbility = GetComponent<SlowTimeAbility>();
        _secondAbility = GetComponent<BounceRocketsAbility>();
        _thirdAbility = GetComponent<ShrapnelAbility>();
    }

    private void Update()
    {
        CheckAndCastAbility(_input.firstAbility, _firstAbility);
        CheckAndCastAbility(_input.secondAbility, _secondAbility);
        CheckAndCastAbility(_input.thirdAbility, _thirdAbility);

        _input.firstAbility = false;
        _input.secondAbility = false;
        _input.thirdAbility = false;
    }

    private void CheckAndCastAbility(bool input, Ability ability)
    {
        if (input && !ability.IsActive && GemCount >= ability.GemCost)
        {
            StartCoroutine(ability.CastAbility());
            GemCount -= ability.GemCost;
            anyAbilityUsed.Invoke();
        }
    }

    public void TakeGem()
    {
        GemCount++;
        gemTaken.Invoke();
    }
}
