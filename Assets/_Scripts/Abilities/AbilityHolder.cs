using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public Ability ability;
    public KeyCode key;
    private PlayerController playerRef;

    float cooldown;
    float duration;

    enum AbilityState { Ready, Active, Cooldown }
    AbilityState state = AbilityState.Ready;

    private void Start()
    {
        playerRef = GetComponent<PlayerController>();
    }

    void Update()
    {
        switch (state)
        {
            case AbilityState.Ready:
                if (Input.GetKeyDown(key) && playerRef.currentStamina >= ability.staminaCost && playerRef.currentMana >= ability.manaCost)
                {
                    ability.Activate(gameObject);
                    state = AbilityState.Active;
                    duration = ability.duration;
                }
            break;
            case AbilityState.Active:
                if (duration > 0)
                {
                    duration -= Time.deltaTime;
                }
                else
                {
                    ability.BeginCooldown(gameObject);
                    state = AbilityState.Cooldown;
                    cooldown = ability.cooldown;
                }
            break;
            case AbilityState.Cooldown:
                if (cooldown > 0)
                {
                    cooldown -= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.Ready;
                }
                break;
        }
    }
}
