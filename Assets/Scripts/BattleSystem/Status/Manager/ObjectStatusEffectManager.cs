using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectStatusEffectManager : MonoBehaviour 
{
	private List<StatusEffectInstance> activeStatusEffects = new List<StatusEffectInstance>();
    
    // Event to notify UI and other systems when status effects change
    public event Action<List<StatusEffectInstance>> OnStatusEffectsChanged;
    
    public List<StatusEffectInstance> ActiveStatusEffects => activeStatusEffects;
    
    private TimedActor actor;
    private IDamagable damagable;
    private void Start()
    {
        if(actor == null)
            actor = GetComponent<TimedActor>();
        if(damagable == null)
            damagable = GetComponent<IDamagable>();
    }

    private void Update()
    {
        UpdateStatusEffects();
    }
    
    private void UpdateStatusEffects()
    {
        bool effectsChanged = false;
        List<StatusEffectInstance> effectsToRemove = new List<StatusEffectInstance>();
    
        foreach (var effect in activeStatusEffects)
        {
            if (!effect.IsActive)
                continue;
            
            // Skip permanent effects for duration check
            if (!effect.Data.isPermanentUntilRemoved)
            {
                effect.RemainingDuration -= Time.deltaTime;
            
                // Check if the effect has expired
                if (effect.RemainingDuration <= 0)
                {
                    effectsToRemove.Add(effect);
                    continue;
                }
            }
        
            // Handle ticking effects
            if (effect.Data.tickInterval > 0)
            {
                effect.TimeSinceLastTick += Time.deltaTime;
            
                if (effect.TimeSinceLastTick >= effect.Data.tickInterval)
                {
                    effect.TimeSinceLastTick = 0;
                
                    // Apply tick effects based on type
                    if (effect.Data.effectType == StatusEffectType.Damage)
                    {
                        if (damagable != null)
                        {
                           effect.ProcessDamageEffect(damagable);
                        }
                    }
                }
            }
        }
    
        // Remove any expired effects
        foreach (var effect in effectsToRemove)
        {
            RemoveStatusEffect(effect.Data.id);
            effectsChanged = true;
        }
    
        if (effectsChanged)
        {
            NotifyStatusEffectsChanged();
        }
    }
    
    
    public void ApplyStatusEffect(StatusEffectData effectData, int stacks = 1)
    {
        // Check if the effectInstance already exists
        StatusEffectInstance existingEffectInstance = activeStatusEffects.FirstOrDefault(e => e.Data.id == effectData.id);
        
        if (existingEffectInstance != null)
        {
            // Handle existing effectInstance according to application rule
            switch (effectData.applicationRule)
            {
                case StatusEffectApplication.Stack:
                    existingEffectInstance.CurrentStacks = Mathf.Min(existingEffectInstance.CurrentStacks + stacks, effectData.maxStacks);
                    AddNewStatusEffect(effectData, stacks);
                    break;
                    
                case StatusEffectApplication.Refresh:
                    existingEffectInstance.RemainingDuration = effectData.duration;
                    break;
                    
                case StatusEffectApplication.Replace:
                    RemoveStatusEffect(existingEffectInstance.Data.id);
                    AddNewStatusEffect(effectData, stacks);
                    return;
            }
            
        }
        else
        {
            
            // Add as a new effectInstance
            AddNewStatusEffect(effectData, stacks);
        }
        
        NotifyStatusEffectsChanged();
    }
    
    private void AddNewStatusEffect(StatusEffectData effectData,int stacks)
    {
        // Create handler instance using factory
        StatusEffectInstance newEffect = StatusEffectHandlerFactory.CreateHandler(
            effectData,stacks);
    
        // Instantiate visual effect if present
        if (effectData.visualEffect != null)
        {
            newEffect.ActiveVisualEffect = Instantiate(effectData.visualEffect, transform.position, Quaternion.identity);
            newEffect.ActiveVisualEffect.transform.SetParent(transform);
        }
    
        // Add to active effects list
        activeStatusEffects.Add(newEffect);
        
        if (actor != null)
        {
            switch (effectData.effectType)
            {
                case StatusEffectType.Stat:
                    newEffect.ApplyStatEffect(actor);
                    break;
                
                case StatusEffectType.Control:
                    newEffect.ApplyControlEffect(actor);
                    break;
                
                case StatusEffectType.Utility:
                    newEffect.ApplyUtilityEffect();
                    break;
            }
        }
    }
    
    public bool RemoveStatusEffect(StatusEffectId statusEffectId, int stacksToRemove = 0)
    {
        StatusEffectInstance effectInstance = activeStatusEffects.FirstOrDefault(e => e.Data.id == statusEffectId);
        
        if (effectInstance == null)
            return false;
            
        // If stacksToRemove is 0 or not specified, remove the effectInstance entirely
        // Otherwise, reduce stacks
        if (stacksToRemove > 0)
        {
            effectInstance.CurrentStacks = Mathf.Max(0, effectInstance.CurrentStacks - stacksToRemove);
            
            // If no stacks remain, remove the effectInstance
            if (effectInstance.CurrentStacks == 0)
            {
                RemoveEffectCompletely(effectInstance);
            }
        }
        else
        {
            RemoveEffectCompletely(effectInstance);
        }
        
        NotifyStatusEffectsChanged();
        return true;
    }
    
    private void RemoveEffectCompletely(StatusEffectInstance effect)
    {
        // Clean up visual effect if present
        if (effect.ActiveVisualEffect != null)
        {
            Destroy(effect.ActiveVisualEffect);
        }
    
        // Apply removal effects
        TimedActor actor = GetComponent<TimedActor>();
        if (actor != null)
        {
            switch (effect.Data.effectType)
            {
                case StatusEffectType.Stat:
                    effect.RemoveStatEffect(actor);
                    break;
                
                case StatusEffectType.Control:
                    effect.RemoveControlEffect(actor);
                    break;
                
                case StatusEffectType.Utility:
                    effect.RemoveUtilityEffect();
                    break;
            }
        }
        activeStatusEffects.Remove(effect);
    }
    
    public void ClearAllStatusEffects()
    {
        foreach (var effect in activeStatusEffects.ToList())
        {
            RemoveStatusEffect(effect.Data.id);
        }
        
        NotifyStatusEffectsChanged();
    }
    
    public bool HasStatusEffect(string statusEffectId)
    {
        //return activeStatusEffects.Any(e => e.Data.id == statusEffectId && e.IsActive);
        return false;
    }
    
    public StatusEffectInstance GetStatusEffect(string statusEffectId)
    {
        //return activeStatusEffects.FirstOrDefault(e => e.Data.id == statusEffectId && e.IsActive);
        return null;
    }
    
    private void NotifyStatusEffectsChanged()
    {
        OnStatusEffectsChanged?.Invoke(activeStatusEffects);
    }
}