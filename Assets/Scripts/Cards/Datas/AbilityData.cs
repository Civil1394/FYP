using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Ability", menuName ="Ability/AbilityData")]
public class AbilityData : ScriptableObject
{
	public string id;
	
	[Header("Type")]
	public AbilityType abilityType;              
	[ConditionalField("abilityType", AbilityType.Projectile)]
	[SerializeField] ProjectileParameter projectileParam;
	
	[ConditionalField("abilityType", AbilityType.Blast)]
	[SerializeField] BlastParameter blastParam;
    
	[ConditionalField("abilityType", AbilityType.Dash)]
	[SerializeField] DashParameter dashParam;

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
	[ConditionalField("abilityType", AbilityType.Projectile,AbilityType.Blast,AbilityType.Dash)]
	[SerializeField] GameObject Object_fx;
	
	public AbilityColorType ColorType;

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
	
	public void TriggerAbility(CasterType casterType, HexDirection castDirection, HexCellComponent casterStandingCell,[CanBeNull]TimeType timeType)
	{
		switch (abilityType)
		{
			case AbilityType.Projectile:
				TriggerProjectile(castDirection, casterStandingCell,timeType);
				break;
			case AbilityType.Blast:
				TriggerBlast(casterType,castDirection,casterStandingCell, timeType);
				break;
			case AbilityType.Dash:
				TriggerDash();
				break;
			default:
				Debug.LogWarning("Effect type not implemented.");
				break;
		}
	}
	private void TriggerProjectile(HexDirection castingDirection, HexCellComponent casterStandingCell,TimeType timeType)
	{
		if (projectileParam != null)
		{
			HexCellComponent spawnCell = BattleManager.Instance.hexgrid.GetCellByDirection(casterStandingCell, castingDirection);
			GameObject bulletObject = Instantiate(Object_fx, spawnCell.transform.position + projectileParam.VFX_Height_Offset, Quaternion.identity);
			var bulletComponent = bulletObject.AddComponent<BulletActor>();
			bulletComponent.InitBullet(
				this.projectileParam,
				castingDirection,
				spawnCell
			);
		}
		else
		{
			Debug.LogError("Projectile parameter not set!");
		}
	}

	private void TriggerBlast(CasterType casterType,HexDirection castingDirection, HexCellComponent casterStandingCell, TimeType timeType)
	{
		if (blastParam != null)
		{
			GameObject blastHandlerObject = new GameObject();
			var blastActor = blastHandlerObject.AddComponent<BlastActor>();
			blastActor.InitBlast(casterType,this.Object_fx,this.blastParam, castingDirection,casterStandingCell);
			

		}
		else
		{
			Debug.LogError("Explosive parameter not set!");
		}
	}

	private void TriggerDash()
	{
		if (dashParam != null)
		{
			Debug.Log("Dash effect triggered");
		}
		else
		{
			Debug.LogError("Dash parameter not set!");
		}
	}
}

public enum AbilityCastType
{
	Auto_targeted,
	Direction_targeted,
	Location_targeted,
	Unit_targeted,
	Self_cast,
}
public enum AbilityTarget
{
	None,
	Player,
	Enemy,
	Environment
}

//Identify who cast the ability
public enum CasterType
{
	Player,
	Enemy,
	None
}
public enum AbilityType
{
	Projectile = 0,
	Blast = 10,
	Dash = 20
}

