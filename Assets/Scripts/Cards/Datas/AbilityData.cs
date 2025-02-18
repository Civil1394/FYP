using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Ability", menuName ="Ability/AbilityData")]
public class AbilityData : ScriptableObject
{
	public string id;
	
	[Header("Type")]
	public AbilityType abilityType;              //WHAT this does?
	[ConditionalField("abilityType", AbilityType.Projectile)]
	[SerializeField] ProjectileParameter projectileParam;

	[FormerlySerializedAs("explosiveParam")]
	[ConditionalField("abilityType", AbilityType.Explosive)]
	[SerializeField] AoeParameter aoeParam;
    
	[ConditionalField("abilityType", AbilityType.Dash)]
	[SerializeField] DashParameter dashParam;
	
	
	[Header("Text")]
	public string Title;

	[TextArea(5, 7)]
	public string Desc;
	
	[Header("Cast")]
	public AbilityCastType CastType; //How the ability is cast?
	
	[Header("Target")]
	public AbilityTarget target;               //WHO is targeted?

	
	
	
    
	[Header("FX")]
	[ConditionalField("abilityType", AbilityType.Projectile,AbilityType.Explosive,AbilityType.Dash)]
	[SerializeField] GameObject Object_fx;
	
	
	public void TriggerAbility(Transform parent, HexDirection castDirection, HexCellComponent casterStandingCell,[CanBeNull]TimeType timeType)
	{
		switch (abilityType)
		{
			case AbilityType.Projectile:
				TriggerProjectile(castDirection, casterStandingCell,timeType);
				break;
			case AbilityType.Explosive:
				TriggerAoe();
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
			Vector3 height_offset = new Vector3(0, 3, 0);
			GameObject bullet = Instantiate(Object_fx, spawnCell.transform.position + height_offset, Quaternion.LookRotation(spawnCell.transform.position));
			var bulletComponent = bullet.AddComponent<BulletActor>();
            
			bulletComponent.Initialize(
				projectileParam.Damage,
				projectileParam.FlowSpeed,
				spawnCell.CellData.Coordinates,
				castingDirection,
				projectileParam.LifeTime,
				height_offset,
				timeType
			);
		}
		else
		{
			Debug.LogError("Projectile parameter not set!");
		}
	}

	private void TriggerAoe()
	{
		if (aoeParam != null)
		{
			Debug.Log($"Explosive effect triggered: {Desc} with radius {aoeParam.Radius}");
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

public enum AbilityType
{
	Projectile = 0,
	Explosive = 10,
	Dash = 20
}
