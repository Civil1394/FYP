using UnityEngine;
using System.Collections;

public class FrostInstance : StatusEffectInstance
{
	
	private float originalActionCooldown;
	private bool effectApplied = false;
    
	public override void ApplyStatEffect(TimedActor actor)
	{
		if (!effectApplied)
		{
			originalActionCooldown = actor.ActionCooldown;
			actor.ActionCooldown = originalActionCooldown * 2.0f;
            
			if (actor.IsTimerActive)
			{
				actor.AdjustCurrentCooldown(2.0f);
			}
            
			effectApplied = true;
			Debug.Log($"Applied Frost effect to {actor.gameObject.name}, cooldown doubled");
		}
	}
    
	public override void RemoveStatEffect(TimedActor actor)
	{
		if (effectApplied)
		{
			actor.ActionCooldown = originalActionCooldown;
            
			if (actor.IsTimerActive)
			{
				actor.AdjustCurrentCooldown(0.5f);
			}
            
			effectApplied = false;
			Debug.Log($"Removed Frost effect from {actor.gameObject.name}, restored cooldown");
		}
	}
}