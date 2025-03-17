using UnityEngine;
using System.Collections;

public class ProjectileVolleyAbilityExecutor: IAbilityExecutor
{
	private GameObject objectFx;
	private ProjectileVolleyParameter parameters;

	public ProjectileVolleyAbilityExecutor(GameObject objectFx, ProjectileVolleyParameter parameters)
	{
		this.objectFx = objectFx;
		this.parameters = parameters;
	}
	public void Execute(CasterType casterType, HexDirection castDirection, HexCellComponent casterStandingCell, TimeType timeType)
	{
		throw new System.NotImplementedException();
	}
}