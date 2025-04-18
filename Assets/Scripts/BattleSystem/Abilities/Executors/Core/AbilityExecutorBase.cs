using UnityEngine;
using System.Collections;

public abstract class AbilityExecutorBase : IAbilityExecutor
{
	protected readonly AbilityData sourceAbility;
	protected readonly GameObject objectFx;
	
	protected AbilityExecutorBase(AbilityData sourceAbility)
	{
		this.sourceAbility = sourceAbility;
		this.objectFx = sourceAbility.Object_fx;
	}
	
	public void Execute(CasterType casterType, HexCellComponent castCell, HexCellComponent casterStandingCell, GameObject casterObject)
	{
		sourceAbility.ApplyStatusEffects(AbilityStatusApplicationType.OnCast, casterObject);
		
		HexDirection castDirection = BattleManager.Instance.hexgrid.GetHexDirectionBy2Cell(casterStandingCell, castCell);
		castCell = casterStandingCell.CellData.GetNeighbor(castDirection).ParentComponent;
		ExecuteAbilitySpecific(casterType, castDirection, castCell, casterStandingCell, casterObject.transform);
	}
	
	protected abstract void ExecuteAbilitySpecific(
		CasterType casterType, 
		HexDirection castDirection,
		HexCellComponent castCell, 
		HexCellComponent casterStandingCell, 
		Transform casterTransform);
}