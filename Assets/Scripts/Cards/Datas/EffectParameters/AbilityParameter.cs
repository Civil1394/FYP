
using UnityEngine;
using UnityEngine.Serialization;

public abstract class AbilityParameter 
{
}
[System.Serializable]
public class DashParameter : AbilityParameter
{
	
}

[System.Serializable]
public class BlastParameter : AbilityParameter
{
	public float Damage;
	
	//public PresetPatternType patternType;
	public int Width;
	public float BlastStepDelay;
	public int BlastStepCount;
	public Vector3 VFX_Height_Offset;
}

[System.Serializable]
public class ProjectileParameter : AbilityParameter
{
	public float Damage;
	[Tooltip("Higher = Faster")]
	public float TravelSpeed;
	public float LifeTime;

	public Vector3 VFX_Height_Offset;
}