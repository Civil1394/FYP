using UnityEngine;
using System.Collections;

public class ProjectileAbilityExecutor : IAbilityExecutor
{
	private GameObject objectFx;
	private ProjectileParameter parameters;
	private IAbilityExecutor abilityExecutorImplementation;

	public ProjectileAbilityExecutor(GameObject objectFx, ProjectileParameter parameters)
	{
		this.objectFx = objectFx;
		this.parameters = parameters;
	}
	
	
	public void Execute(CasterType casterType, HexCellComponent castCell, HexCellComponent casterStandingCell, GameObject casterObject,
		TimeType timeType)
	{
		var castDirection = BattleManager.Instance.hexgrid.GetHexDirectionBy2Cell(casterStandingCell, castCell);
		//HexCellComponent spawnCell = BattleManager.Instance.hexgrid.GetCellByDirection(casterStandingCell, castCell.ce);
		GameObject bulletObject = Object.Instantiate(objectFx, castCell.transform.position + parameters.VFX_Height_Offset, Quaternion.identity);
		var bulletComponent = bulletObject.AddComponent<ProjectileActor>();
		bulletComponent.InitBullet(
			casterType,
			parameters,
			castDirection,
			castCell
		);
	}
}