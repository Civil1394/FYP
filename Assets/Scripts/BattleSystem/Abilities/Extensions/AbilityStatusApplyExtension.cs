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
	public StatusEffectData statusEffect;//Data of status Effect
    
    //Below are the application param of status effect
	public AbilityStatusApplicationType applicationType;
	public int initialStacks = 1;
    [Tooltip("1.0 = 100% chance")]
	public float applicationChance = 1.0f; 
	[Tooltip("If true, will attempt to apply to self instead of target")]
	public bool applyToSelf = false;
}

[System.Serializable]
public class StatusEffectsParameter : AbilityParameter
{
    //list of the the applicable status effect of the ability
	public List<StatusApplicationParameter> statusEffects = new List<StatusApplicationParameter>();
}

//Extension Methods for Abilitydata
public static class AbilityDataExtensions
{
    public static void ApplyStatusEffects(this AbilityData ability, 
                                          AbilityStatusApplicationType applicationType, 
                                          GameObject target)
    {
        // Check if the ability has a StatusEffectsParameter
        if (ability == null || target == null)
            return;
        
        StatusEffectsParameter statusParam  = ability.statusEffectsParam;
        
        if (statusParam == null || statusParam.statusEffects.Count == 0)
            return;
        
        // Apply each applicable status effect
        foreach (var statusApp in statusParam.statusEffects)
        {
            // Skip if this isn't the right application type
            if (statusApp.applicationType != applicationType)
                continue;
                
            // Check chance to apply
            if (statusApp.applicationChance < 1.0f && Random.value > statusApp.applicationChance)
                continue;
            
            ObjectStatusEffectManager targetStatusManager = target.GetComponent<ObjectStatusEffectManager>();
    
            if (targetStatusManager == null)
            {
                targetStatusManager = target.AddComponent<ObjectStatusEffectManager>();
            }
        
            if (targetStatusManager != null)
            {
                // Apply to the target
                targetStatusManager.ApplyStatusEffect(statusApp.statusEffect, statusApp.initialStacks);
            }
           
        }
    }
}