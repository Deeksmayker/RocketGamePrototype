using System;
using Assets.Scripts.Model.Interfaces;
using Player;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GameInputManager))]
public class PlayerAbilities : MonoBehaviour, ITakeGem
{
    [field: SerializeField] public int GemCount { get; private set; }

    [NonSerialized] public Ability FirstAbility, SecondAbility, ThirdAbility;

    private GameInputManager _input;

    public UnityEvent gemTaken = new();
    public UnityEvent anyAbilityUsed = new();

    private void Awake()
    {
        _input = GetComponent<GameInputManager>();
        FirstAbility = GetComponent<SlowTimeAbility>();
        SecondAbility = GetComponent<BounceRocketsAbility>();
        ThirdAbility = GetComponent<ExplosionRadiusAbility>();
    }

    private void Update()
    {
        CheckAndCastAbility(_input.firstAbility, FirstAbility);
        CheckAndCastAbility(_input.secondAbility, SecondAbility);
        CheckAndCastAbility(_input.thirdAbility, ThirdAbility);

        _input.firstAbility = false;
        _input.secondAbility = false;
        _input.thirdAbility = false;
    }

    private void CheckAndCastAbility(bool input, Ability ability)
    {
        if (input && !ability.IsActive && GemCount >= ability.GemCost)
        {
            GemCount -= ability.GemCost;
            StartCoroutine(ability.CastAbility());
            anyAbilityUsed.Invoke();
        }
    }

    public void TakeGem()
    {
        GemCount++;
        gemTaken.Invoke();
    }
}
