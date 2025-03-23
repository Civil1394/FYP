using UnityEngine;
using System.Collections;

public class BlastAbilityExecutor : IAbilityExecutor
{
	private GameObject objectFx;
	private BlastParameter parameters;
	private AbilityData sourceAbility;

	
	public BlastAbilityExecutor(GameObject objectFx, BlastParameter parameters, AbilityData sourceAbility)
	{
		this.objectFx = objectFx;
		this.parameters = parameters;
		this.sourceAbility = sourceAbility;
	}

	public void Execute(CasterType casterType, HexDirection castingDirection, HexCellComponent casterStandingCell,GameObject casterObject, TimeType timeType)
	{
		GameObject blastHandlerObject = new GameObject();
		var blastActor = blastHandlerObject.AddComponent<BlastActor>();
		blastActor.InitBlast(casterType,objectFx,parameters, castingDirection,casterStandingCell);
		
		// Apply on-cast status effects
		if (sourceAbility != null)
		{
			if(casterType == CasterType.Player)
			{
				sourceAbility.ApplyStatusEffects(AbilityStatusApplicationType.OnCast, BattleManager.Instance.PlayerActorInstance.gameObject, BattleManager.Instance.PlayerActorInstance.gameObject, casterType);
			}
		}
	}
}