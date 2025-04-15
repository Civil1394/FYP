using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Ability", menuName ="Ability/AbilityData")]
public partial class AbilityData : ScriptableObject
{
	public string id;
	
	[Header("Type")]
	public AbilityType abilityType;         
	
	[ConditionalField("abilityType", AbilityType.Projectile,AbilityType.LocationalProjectile)]
	public ProjectileParameter projectileParam;
	
	[ConditionalField("abilityType", AbilityType.ProjectileVolley)]
	public ProjectileVolleyParameter projectileVolleyParam;
	
	[ConditionalField("abilityType", AbilityType.Blast)]
	public BlastParameter blastParam;
	
	[ConditionalField("abilityType", AbilityType.Dash)]
	public DashParameter dashParam;
	
	[ConditionalField("abilityType",AbilityType.ExplosiveCharge)]
	public ExplosiveChargeParameter explosiveChargeParam;
	
	
	public int PrerequisiteChargeSteps;
	[Header("Text")]
	public string Title;

	[TextArea(5, 7)]
	public string Desc;
	
	[Header("Icon")]
	public Sprite Icon;
	
	[Header("Cast")]
	public AbilityCastType CastType;
	[ConditionalField("CastType",AbilityCastType.Direction_targeted,AbilityCastType.Location_targeted)]
	public HexPatternBase SelectablePattern;
	[ConditionalField("CastType",AbilityCastType.Location_targeted)]
	public HexPatternBase AoePattern;
	
	[Header("FX")]
	[ConditionalField("abilityType", AbilityType.Projectile,AbilityType.ProjectileVolley,AbilityType.Blast,AbilityType.Dash,AbilityType.LocationalProjectile)]
	public GameObject Object_fx;
	
	public AbilityColorType ColorType{
		get;
		set;
	}

	
	/// <summary>
	/// Creates a new AbilityData instance with a specified color type.
	/// </summary>
	public AbilityData Create()
	{
		AbilityData instance = Instantiate(this);

		instance.ColorType = AbilityColorTypeInitializer.GetAbilityColorType(abilityType);
		return instance;
	}

	public void TriggerAbility(CasterType casterType, HexCellComponent castCell, HexCellComponent casterStandingCell,
		GameObject casterObject)

	{
		IAbilityExecutor executor = AbilityExecutorFactory.CreateExecutor(this);
		if (executor != null)
		{
			executor.Execute(casterType, castCell, casterStandingCell, casterObject);
		}
		else
		{
			Debug.LogWarning($"No executor found for ability type: {abilityType}");
		}
	}
}


