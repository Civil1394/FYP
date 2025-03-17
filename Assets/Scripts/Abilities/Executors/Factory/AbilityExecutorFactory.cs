using UnityEngine;
using System.Collections;

public static class AbilityExecutorFactory
{
	public static IAbilityExecutor CreateExecutor(AbilityData abilityData)
	{
		switch (abilityData.abilityType)
		{
			case AbilityType.Projectile:
				return new ProjectileAbilityExecutor(abilityData.Object_fx, abilityData.projectileParam);
			case AbilityType.Blast:
				return new BlastAbilityExecutor(abilityData.Object_fx, abilityData.blastParam);
			case AbilityType.ProijectileVolley:
				return new ProjectileVolleyAbilityExecutor(abilityData.Object_fx, abilityData.projectileVolleyParam);
			default:
				Debug.LogWarning($"No executor implemented for ability type: {abilityData.abilityType}");
				return null;
		}
	}
}