using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerController))]
public abstract class Ability : MonoBehaviour
{
    [field: SerializeField] public int GemCost { get; protected set; }
    [SerializeField] protected float duration;

    protected PlayerController playerController;

    public UnityEvent abilityEnded = new();

    protected virtual void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public abstract IEnumerator CastAbility();
}
