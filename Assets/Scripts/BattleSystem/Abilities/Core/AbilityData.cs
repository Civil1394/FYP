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
	
	public bool IsRandomColorType;
	[Header("FX")]
	[ConditionalField("IsRandomColorType", false)]
	public AbilityColorType ColorType;
	
	/// <summary>
	/// Creates a new AbilityData instance with a specified color type.
	/// </summary>
	/// <param name="isRandom">If true, selects a random color type.</param>
	/// <param name="color">Optional: Specifies the ability color. If null, a random color will be chosen.</param>
	/// <returns>Returns a new instance of AbilityData.</returns>
	public AbilityData Create()
	{
		AbilityData ability = Instantiate(this);

		if (IsRandomColorType)
		{
			ability.ColorType = (AbilityColorType)Random.Range(0, 6);	
		}
		return ability;
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


