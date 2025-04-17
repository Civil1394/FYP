using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LocationalProjectileActor : DamageActor 
{
	private ProjectileParameter parameter;
	public HexCellComponent CasterCell{get; private set;}

	public HexCellComponent TargetCell{get; private set;}

	public override event Action<GameObject> OnHitApplyStatusEffect;

	public void InitBullet(AbilityData ad, CasterType casterType, ProjectileParameter parameter,
		HexCellComponent casterCell, HexCellComponent targetCell)
	{
		abilityData = ad;

		this.casterType = casterType;
		this.gameObject.tag = "DamageActor";
		this.parameter = parameter;
		
		_damage = parameter.Damage;
		CasterCell = casterCell;
		TargetCell = targetCell;
		if (targetCell != null)
		{
			Launch();
		}
	}

	private void Launch()
	{
		var dir = TargetCell.transform.position - transform.position;
		transform.right = dir;
		var tempDis = Vector3Int.Distance(CasterCell.CellData.Coordinates, TargetCell.CellData.Coordinates);
		TargetCell.HighLightCell(abilityData.ColorType);
		transform.DOMove(TargetCell.transform.position, 0.05f * tempDis).SetEase(Ease.Linear).OnComplete(()=>Destroy(gameObject));
	}
	public override void DoDamage(Action<float> damageAction, GameObject damagedTarget, GameObject source = null)
	{
		throw new NotImplementedException();
	}

	private void OnDestroy()
	{
		TargetCell.UnhighLightCell();
	}
}