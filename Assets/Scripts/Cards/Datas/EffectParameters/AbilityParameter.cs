
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
public class AoeParameter : AbilityParameter
{
	[FormerlySerializedAs("PatternType")] public PresetPatternType patternType;
	public int Radius;
	public float Damage;
}

[System.Serializable]
public class ProjectileParameter : AbilityParameter
{
	public float Damage;
	[Tooltip("Higher = Faster")]
	public float TravelSpeed;
	public float LifeTime;
}