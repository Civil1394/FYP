using UnityEngine;
using System.Collections;

public static class AbilityExecutorFactory
{
	public static IAbilityExecutor CreateExecutor(AbilityData abilityData)
	{
		switch (abilityData.abilityType)
		{
			case AbilityType.Projectile:
				return new ProjectileAbilityExecutor(abilityData);
                
			case AbilityType.Blast:
				return new BlastAbilityExecutor(abilityData);
                
			case AbilityType.ProjectileVolley:
				return new ProjectileVolleyAbilityExecutor(abilityData);
			case AbilityType.Dash:
				return new DashAbilityExecutor(abilityData);
			case AbilityType.LocationalProjectile:
				return new LocationalProjectileAbilityExecuter(abilityData);
			default:
				Debug.LogWarning($"No executor implemented for ability type: {abilityData.abilityType}");
				return null;
		}
	}
}