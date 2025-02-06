using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class HourGlassControllerBase : MonoBehaviour  
{
	protected abstract List<AbilityData> passiveEffects { get; }
	protected abstract List<int> triggerThreshold { get; }


	protected abstract void OnThresholdReached();
	
}