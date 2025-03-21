using UnityEngine;
using System.Collections;

public class ProjectileAbilityExecutor : IAbilityExecutor
{
	private GameObject objectFx;
	private ProjectileParameter parameters;

	public ProjectileAbilityExecutor(GameObject objectFx, ProjectileParameter parameters)
	{
		this.objectFx = objectFx;
		this.parameters = parameters;
	}
	

	public void Execute(CasterType casterType, HexDirection castDirection, HexCellComponent casterStandingCell, TimeType timeType)
	{
		HexCellComponent spawnCell = BattleManager.Instance.hexgrid.GetCellByDirection(casterStandingCell, castDirection);
		GameObject bulletObject = Object.Instantiate(objectFx, spawnCell.transform.position + parameters.VFX_Height_Offset, Quaternion.identity);
		var bulletComponent = bulletObject.AddComponent<ProjectileActor>();
		bulletComponent.InitBullet(
			casterType,
			parameters,
			castDirection,
			spawnCell
		);
	}
}