using UnityEngine;
using System.Collections;

public partial class AbilityData : ScriptableObject
{
	[Header("Status Effects")]
	[ConditionalField("abilityType", AbilityType.Projectile, AbilityType.ProjectileVolley, AbilityType.Blast)]
	public StatusEffectData statusEffectdata;
}