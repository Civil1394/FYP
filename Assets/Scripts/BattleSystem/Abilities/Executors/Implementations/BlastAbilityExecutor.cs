using UnityEngine;
using System.Collections;

public class BlastAbilityExecutor : IAbilityExecutor
{
	private GameObject objectFx;
	private BlastParameter parameters;

	public BlastAbilityExecutor(GameObject objectFx, BlastParameter parameters)
	{
		this.objectFx = objectFx;
		this.parameters = parameters;
	}

	public void Execute(CasterType casterType, HexDirection castingDirection, HexCellComponent casterStandingCell, TimeType timeType)
	{
		GameObject blastHandlerObject = new GameObject();
		var blastActor = blastHandlerObject.AddComponent<BlastActor>();
		blastActor.InitBlast(casterType,objectFx,parameters, castingDirection,casterStandingCell);

	}
}