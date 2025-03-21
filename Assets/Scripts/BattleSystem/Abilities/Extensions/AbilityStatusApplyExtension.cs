using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AbilityStatusApplicationType
{
	OnHit,      // Apply status when the ability hits a target
	OnCast,     // Apply status when the ability is cast
	OverTime    // Apply status over time (for AoE effects)
}

[System.Serializable]
public class StatusApplicationParameter
{
	public StatusEffectData statusEffect;
	public AbilityStatusApplicationType applicationType;
	public int initialStacks = 1;
	public float applicationChance = 1.0f; // 1.0 = 100% chance
	[Tooltip("If true, will attempt to apply to self instead of target")]
	public bool applyToSelf = false;
}

[System.Serializable]
public class StatusEffectParameter : AbilityParameter
{
	public List<StatusApplicationParameter> statusEffects = new List<StatusApplicationParameter>();
}


public static class AbilityDataExtensions
{
    public static void ApplyStatusEffects(this AbilityData ability, 
                                          AbilityStatusApplicationType applicationType,
                                          GameObject source, 
                                          GameObject target,
                                          CasterType casterType)
    {
        // Check if the ability has a StatusEffectParameter
        if (ability == null || target == null)
            return;
        
        StatusEffectParameter statusParam = null;
        
        // Use reflection to get the field if it exists
        var field = ability.GetType().GetField("statusEffectParam");
        if (field != null)
        {
            statusParam = field.GetValue(ability) as StatusEffectParameter;
        }
        
        if (statusParam == null || statusParam.statusEffects.Count == 0)
            return;
        //Add status effect manager component to object
        ObjectStatusEffectManager targetStatusManager = target.GetComponent<ObjectStatusEffectManager>();
        ObjectStatusEffectManager sourceStatusManager = source.GetComponent<ObjectStatusEffectManager>();
        
        if (targetStatusManager == null)
        {
            targetStatusManager = target.AddComponent<ObjectStatusEffectManager>();
        }
        
        if (sourceStatusManager == null && source != null)
        {
            sourceStatusManager = source.AddComponent<ObjectStatusEffectManager>();
        }
        
        // Apply each applicable status effect
        foreach (var statusApp in statusParam.statusEffects)
        {
            // Skip if this isn't the right application type
            if (statusApp.applicationType != applicationType)
                continue;
                
            // Check chance to apply
            if (statusApp.applicationChance < 1.0f && Random.value > statusApp.applicationChance)
                continue;
                
            if (statusApp.applyToSelf && sourceStatusManager != null)
            {
                // Apply to the caster
                sourceStatusManager.ApplyStatusEffect(statusApp.statusEffect, target, casterType, statusApp.initialStacks);
            }
            else if (targetStatusManager != null)
            {
                // Apply to the target
                targetStatusManager.ApplyStatusEffect(statusApp.statusEffect, source, casterType, statusApp.initialStacks);
            }
        }
    }
}