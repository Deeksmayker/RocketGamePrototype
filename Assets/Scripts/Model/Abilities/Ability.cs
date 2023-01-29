using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Ability : MonoBehaviour
{
    [field: SerializeField] public int GemCost { get; protected set; }
    [SerializeField] protected float duration;
    public bool IsActive { get; private set; }

    protected virtual void Awake()
    {
        abilityCasted.AddListener(() => IsActive = true);
        abilityEnded.AddListener(() => IsActive = false);
    }

    protected void OnEnable()
    {
        LoadUpgradedValue();
    }

    public UnityEvent abilityCasted = new();
    public UnityEvent abilityEnded = new();

    public abstract IEnumerator CastAbility();
    public abstract void DisableAbility();

    public abstract void LoadUpgradedValue();

    protected virtual void OnDisable()
    {
        DisableAbility();
    }
}
