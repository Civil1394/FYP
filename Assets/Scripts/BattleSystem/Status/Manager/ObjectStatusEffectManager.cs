using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectStatusEffectManager : MonoBehaviour 
{
	private List<StatusEffect> activeStatusEffects = new List<StatusEffect>();
    
    // Event to notify UI and other systems when status effects change
    public event Action<List<StatusEffect>> OnStatusEffectsChanged;
    
    // Event for when a specific status effect is applied
    public event Action<StatusEffect> OnStatusEffectApplied;
    
    // Event for when a specific status effect is removed
    public event Action<StatusEffect> OnStatusEffectRemoved;
    
    // Event that passes damage values when taking damage from a status effect
    public event Action<float, StatusEffectData> OnStatusEffectDamage;
    
    public List<StatusEffect> ActiveStatusEffects => activeStatusEffects;
    
    private void Update()
    {
        UpdateStatusEffects();
    }
    
    private void UpdateStatusEffects()
    {
        bool effectsChanged = false;
        List<StatusEffect> effectsToRemove = new List<StatusEffect>();
        
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
                    ProcessStatusEffectTick(effect);
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
    
    private void ProcessStatusEffectTick(StatusEffect effect)
    {
        switch (effect.Data.effectType)
        {
            case StatusEffectType.Damage:
                float damage = effect.CalculateValue();
                OnStatusEffectDamage?.Invoke(damage, effect.Data);
                break;
                
            case StatusEffectType.Stat:
                // Stats are mostly handled passively through modifiers
                break;
                
            case StatusEffectType.Control:
                // Control effects typically don't tick but apply continuous state
                break;
                
            case StatusEffectType.Utility:
                // Custom behavior for utility effects
                break;
        }
    }
    
    public void ApplyStatusEffect(StatusEffectData effectData, GameObject source, CasterType casterType, int stacks = 1)
    {
        // Check if the effect already exists
        StatusEffect existingEffect = activeStatusEffects.FirstOrDefault(e => e.Data.id == effectData.id);
        
        if (existingEffect != null)
        {
            // Handle existing effect according to application rule
            switch (effectData.applicationRule)
            {
                case StatusEffectApplication.Stack:
                    existingEffect.CurrentStacks = Mathf.Min(existingEffect.CurrentStacks + stacks, effectData.maxStacks);
                    break;
                    
                case StatusEffectApplication.Refresh:
                    existingEffect.RemainingDuration = effectData.duration;
                    break;
                    
                case StatusEffectApplication.Replace:
                    RemoveStatusEffect(existingEffect.Data.id);
                    AddNewStatusEffect(effectData, source, casterType, stacks);
                    return;
            }
            
            OnStatusEffectApplied?.Invoke(existingEffect);
        }
        else
        {
            // Add as a new effect
            AddNewStatusEffect(effectData, source, casterType, stacks);
        }
        
        NotifyStatusEffectsChanged();
    }
    
    private void AddNewStatusEffect(StatusEffectData effectData, GameObject source, CasterType casterType, int stacks)
    {
        StatusEffect newEffect = effectData.CreateInstance(gameObject, source, casterType, stacks);
        
        // Instantiate visual effect if present
        if (effectData.visualEffect != null)
        {
            newEffect.ActiveVisualEffect = Instantiate(effectData.visualEffect, transform.position, Quaternion.identity);
            newEffect.ActiveVisualEffect.transform.SetParent(transform);
        }
        
        activeStatusEffects.Add(newEffect);
        OnStatusEffectApplied?.Invoke(newEffect);
    }
    
    public bool RemoveStatusEffect(string statusEffectId, int stacksToRemove = 0)
    {
        StatusEffect effect = activeStatusEffects.FirstOrDefault(e => e.Data.id == statusEffectId);
        
        if (effect == null)
            return false;
            
        // If stacksToRemove is 0 or not specified, remove the effect entirely
        // Otherwise, reduce stacks
        if (stacksToRemove > 0)
        {
            effect.CurrentStacks = Mathf.Max(0, effect.CurrentStacks - stacksToRemove);
            
            // If no stacks remain, remove the effect
            if (effect.CurrentStacks == 0)
            {
                RemoveEffectCompletely(effect);
            }
        }
        else
        {
            RemoveEffectCompletely(effect);
        }
        
        NotifyStatusEffectsChanged();
        return true;
    }
    
    private void RemoveEffectCompletely(StatusEffect effect)
    {
        effect.IsActive = false;
        
        // Clean up visual effect
        if (effect.ActiveVisualEffect != null)
        {
            Destroy(effect.ActiveVisualEffect);
        }
        
        activeStatusEffects.Remove(effect);
        OnStatusEffectRemoved?.Invoke(effect);
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
        return activeStatusEffects.Any(e => e.Data.id == statusEffectId && e.IsActive);
    }
    
    public StatusEffect GetStatusEffect(string statusEffectId)
    {
        return activeStatusEffects.FirstOrDefault(e => e.Data.id == statusEffectId && e.IsActive);
    }
    
    private void NotifyStatusEffectsChanged()
    {
        OnStatusEffectsChanged?.Invoke(activeStatusEffects);
    }
}