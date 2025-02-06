using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class HourGlassControllerBase : MonoBehaviour  
{
	protected abstract List<AbilityData> passiveEffects { get; }
	protected abstract List<int> triggerThreshold { get; }


	protected abstract void OnThresholdReached();
	
}

public class EnemyHourGlassControllerBaseImpl : HourGlassControllerBase
{
	protected override List<AbilityData> passiveEffects { get; }
	protected override List<int> triggerThreshold { get; }
	protected override void OnThresholdReached()
	{
		throw new System.NotImplementedException();
	}
}

public class PlayerHourGlassControllerBaseImpl : HourGlassControllerBase
{
	protected override List<AbilityData> passiveEffects { get; }
	protected override List<int> triggerThreshold { get; }
	protected override void OnThresholdReached()
	{
		throw new System.NotImplementedException();
	}
}