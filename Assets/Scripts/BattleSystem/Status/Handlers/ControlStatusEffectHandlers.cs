using UnityEngine;
using System.Collections;

public class FrostInstance : StatusEffectInstance
{
	
	private float originalActionCooldown;
	private bool effectApplied = false;
    
	public override void ApplyControlEffect(TimedActor actor)
	{
		if (!effectApplied)
		{
			actor.UpdateActionCooldownMultiplier(Data.baseValue * Data.scalingFactor,false);
            
			// if (actor.IsTimerActive)
			// {
			// 	actor.AdjustCurrentCooldown(Data.baseValue * Data.scalingFactor);
			// }
            
			effectApplied = true;
			Debug.Log($"Applied Slowness effect to {actor.gameObject.name}, cooldown doubled");
		}
	}
    
	public override void RemoveControlEffect(TimedActor actor)
	{
		if (effectApplied)
		{
            
			actor.UpdateActionCooldownMultiplier(Data.baseValue * Data.scalingFactor,true);
			effectApplied = false;
			Debug.Log($"Removed Slowness effect from {actor.gameObject.name}, restored cooldown");
		}
	}
}