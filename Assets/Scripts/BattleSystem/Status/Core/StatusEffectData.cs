using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum StatusEffectId
{
    None = 0,
    
    Poison = 5,
    
    Slowness = 10,
}
public enum StatusEffectType
{
    Damage,     // Deals damage over time (e.g., Poison, Burn)
    Control,    // Affects movement or actions (e.g., Stun, Root)
    Stat,       // Modifies stats (e.g., Weakness, Strength)
    Utility     // Other effects (e.g., Reveal, Mark)
}

public enum StatusEffectApplication
{
    Stack,      // Multiple instances stack (e.g., Poison stacks)
    Refresh,    // New application refreshes duration
    Replace     // New application replaces old one
}

[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "Ability/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    [Header("Identification")]
    public StatusEffectId id;
    public string displayName;
    public Sprite icon;
    
    [Header("Effect Settings")]
    public StatusEffectType effectType;
    public StatusEffectApplication applicationRule;
    
    [Header("Timing")]
    public float duration = 5f;
    public float tickInterval = 1f;
    public bool isPermanentUntilRemoved = false;
    
    [Header("Visual")]
    public GameObject visualEffect;
    
    [Header("Parameters")]
    [Tooltip("Base value for the effect (damage per tick, stat modification amount, etc.)")]
    public float baseValue = 10f;
    [Tooltip("Scaling factor for the effect (if applicable)")]
    public float scalingFactor = 1.0f;
    [Tooltip("Maximum number of stacks")]
    [ConditionalField("applicationRule", StatusEffectApplication.Stack)]
    public int maxStacks = 1;
    
    [TextArea(3, 5)]
    public string description;
    
}

// Runtime instance of a status effect
[System.Serializable]
public class StatusEffectInstance : IStatusEffectHandler
{
    public StatusEffectData Data;
    public float RemainingDuration;
    public int CurrentStacks;
    public float TimeSinceLastTick;
    public bool IsActive;
    
    public GameObject ActiveVisualEffect;
    
    public virtual float ProcessDamageEffect(IDamagable damagable)
    {
        throw new System.NotImplementedException("This handler does not support damage effects");
    }
    
    public virtual void ApplyControlEffect(TimedActor actor)
    {
        throw new System.NotImplementedException("This handler does not support control effects");
    }
    
    public virtual void ApplyStatEffect(TimedActor actor)
    {
        throw new System.NotImplementedException("This handler does not support stat effects");
    }
    
    public virtual void ApplyUtilityEffect()
    {
        throw new System.NotImplementedException("This handler does not support utility effects");
    }

    public virtual void RemoveControlEffect(TimedActor actor)
    {
        throw new NotImplementedException();
    }

    public virtual void RemoveStatEffect(TimedActor actor)
    {
        throw new NotImplementedException();
    }

    public virtual void RemoveUtilityEffect()
    {
        throw new NotImplementedException();
    }
    
}

