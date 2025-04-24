using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;

public class LocationalProjectileActor : DamageActor 
{
	private LocationalProjectileParameter parameter;
	public HexCellComponent CasterCell{get; private set;}

	public HexCellComponent TargetCell{get; private set;}
	
	private List<HexCellComponent> highlightedCells = new List<HexCellComponent>();
	public override event Action<GameObject> OnHitApplyStatusEffect;

	public void InitBullet(AbilityData ad, CasterType casterType, LocationalProjectileParameter parameter,
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
		highlightedCells.Add(TargetCell);
		transform.DOMove(TargetCell.transform.position, 0.1f * tempDis).SetEase(Ease.InQuad).OnComplete(() =>
		{
			Instantiate(parameter.BlastVFX,TargetCell.transform.position,Quaternion.identity);
			Destroy(gameObject);
		});
	}
	public override void DoDamage(Action<float> damageAction, GameObject damagedTarget, GameObject source = null)
	{
		damageAction?.Invoke(_damage);
		Destroy(gameObject);
		OnHitApplyStatusEffect?.Invoke(damagedTarget);
	}

	private void OnDestroy()
	{

		foreach (var c in highlightedCells)
		{
			c.UnhighLightCell();
		}
	}
}