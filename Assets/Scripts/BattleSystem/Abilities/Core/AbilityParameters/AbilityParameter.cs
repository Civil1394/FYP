
using UnityEngine;
using UnityEngine.Serialization;

public abstract class AbilityParameter 
{
}


[System.Serializable]
public class BlastParameter : AbilityParameter
{
	public float Damage;
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
	
	public Vector3 VFX_Height_Offset = new Vector3(0, 3, 0);
	
	public bool IsSelfDestructOnCollision = true;
}

[System.Serializable]
public class ProjectileVolleyParameter : AbilityParameter
{
	[Tooltip("Configuration for individual projectiles (damage, speed, lifetime, etc.)")]
	public ProjectileParameter ProjectileConfig;

	[Tooltip("Number of projectiles fired in a single cast")]
	public int ProjectilePerBurst = 1;

	[Tooltip("Delay between firing each projectile within a cast (0 = all at once)")]
	public float DelayBetweenProjectiles = 0f;

	[Tooltip("Total number of times this volley is repeated (e.g., 3 bursts)")]
	public int BurstCount = 1;

	[Tooltip("Delay between each cast of the volley")]
	public float DelayBetweenBurst = 0f;
}

[System.Serializable]
public class ExplosiveChargeParameter : AbilityParameter
{
	public float Damage;
	public float ChargeTime;
	public int TriggerCount;
	public float TriggerBetweenDelay;
}

[System.Serializable]
public class DashParameter : AbilityParameter
{
	
}
[System.Serializable]
public class UtilityParameter : AbilityParameter
{
	
}

