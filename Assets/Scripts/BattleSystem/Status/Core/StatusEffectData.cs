using UnityEngine;
using System.Collections.Generic;

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
    public string id;
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
    
    // Create a runtime instance of this status effect with specific parameters
    public StatusEffect CreateInstance(GameObject target, GameObject source, CasterType casterType, int initialStacks = 1)
    {
        StatusEffect statusEffect = new StatusEffect
        {
            Data = this,
            Target = target,
            Source = source,
            CasterType = casterType,
            RemainingDuration = duration,
            CurrentStacks = Mathf.Min(initialStacks, maxStacks),
            TimeSinceLastTick = 0f,
            IsActive = true
        };
        
        return statusEffect;
    }
}

// Runtime instance of a status effect
[System.Serializable]
public class StatusEffect
{
    public StatusEffectData Data;
    public GameObject Target;
    public GameObject Source;
    public CasterType CasterType;
    public float RemainingDuration;
    public int CurrentStacks;
    public float TimeSinceLastTick;
    public bool IsActive;
    
    public GameObject ActiveVisualEffect;
    
    public float CalculateValue()
    {
        return Data.baseValue * Data.scalingFactor * CurrentStacks;
    }
}

