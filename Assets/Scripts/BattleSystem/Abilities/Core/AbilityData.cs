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
	[ConditionalField("abilityType", AbilityType.Projectile)]
	public ProjectileParameter projectileParam;
	
	[ConditionalField("abilityType", AbilityType.ProjectileVolley)]
	public ProjectileVolleyParameter projectileVolleyParam;
	
	[ConditionalField("abilityType", AbilityType.Blast)]
	public BlastParameter blastParam;
	
	[ConditionalField("abilityType", AbilityType.Dash)]
	public DashParameter dashParam;

	public int PrerequisiteChargeSteps;
	[Header("Text")]
	public string Title;

	[TextArea(5, 7)]
	public string Desc;
	
	[Header("Icon")]
	public Sprite Icon;
	[Header("Cast")]
	public AbilityCastType CastType;
    
	[Header("FX")]
	[ConditionalField("abilityType", AbilityType.Projectile,AbilityType.ProjectileVolley,AbilityType.Blast,AbilityType.Dash)]
	public GameObject Object_fx;
	
	public AbilityColorType ColorType;

	public HexPatternBase selectablePattern;
	public HexPatternBase aoePattern;
	/// <summary>
	/// Creates a new AbilityData instance with a specified color type.
	/// </summary>
	/// <param name="isRandom">If true, selects a random color type.</param>
	/// <param name="color">Optional: Specifies the ability color. If null, a random color will be chosen.</param>
	/// <returns>Returns a new instance of AbilityData.</returns>
	public AbilityData Create(AbilityData bp, bool isRandom, AbilityColorType? color = null)
	{
		if (bp == null) return null;
		
		AbilityData ability = Instantiate(bp);

		if (!isRandom)
		{
			ability.ColorType = color ?? (AbilityColorType)Random.Range(0, 3);
		}
		else
		{
			ability.ColorType = (AbilityColorType)Random.Range(0, 3);
		}

		return ability;
	}
	public void TriggerAbility(CasterType casterType, HexDirection castDirection, HexCellComponent casterStandingCell,GameObject casterObject, TimeType timeType)
	{
		IAbilityExecutor executor = AbilityExecutorFactory.CreateExecutor(this);
		if (executor != null)
		{
			executor.Execute(casterType, castDirection, casterStandingCell,casterObject, timeType);
		}
		else
		{
			Debug.LogWarning($"No executor found for ability type: {abilityType}");
		}
	}
}


