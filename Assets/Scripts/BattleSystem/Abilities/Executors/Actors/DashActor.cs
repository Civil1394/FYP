using System;
using UnityEngine;
using System.Collections;

public class DashActor : DamageActor 
{
	private AbilityData abilityData;

	private DashParameter parameter;

	public HexCellComponent TargetCell{get; private set;}
	public float DashSpeed { get; private set; }
	public override event Action<GameObject> OnHitApplyStatusEffect;

	public void InitDash(AbilityData ad, CasterType casterType, DashParameter parameter, HexCellComponent targetCell,
		TimedActor casterActor)
	{
		abilityData = ad;

		this.casterType = casterType;
		this.gameObject.tag = "DamageActor";
		this.parameter = parameter;
        
        
		_damage = parameter.Damage;
        
		TargetCell = targetCell;

		if (targetCell != null)
		{
			//Launch when init 
			switch (casterType)
			{
				case CasterType.Player:
					PlayerActor playerCaster = casterActor as PlayerActor;
					LaunchPlayer(playerCaster);
					break;
				case CasterType.Enemy:
					EnemyActor enemyCaster = casterActor as EnemyActor;
					LaunchEnemy(enemyCaster);
					break;
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void LaunchEnemy(EnemyActor enemyActor)
	{
		enemyActor.ExecuteDash(TargetCell);
	}

	void LaunchPlayer(PlayerActor playerActor)
	{
		playerActor.ExecuteDash(TargetCell);
	}
	public override void DoDamage(Action<float> damageAction, GameObject damagedTarget, GameObject source = null)
	{
		throw new NotImplementedException();
	}
}