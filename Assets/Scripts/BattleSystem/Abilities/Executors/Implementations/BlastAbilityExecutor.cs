using UnityEngine;
using System.Collections;

public class BlastAbilityExecutor : AbilityExecutorBase
{
	private readonly BlastParameter parameters;
    
	public BlastAbilityExecutor(AbilityData sourceAbility) : base(sourceAbility)
	{
		this.parameters = sourceAbility.blastParam;
	}
    
	protected override void ExecuteAbilitySpecific(
		CasterType casterType, 
		HexDirection castDirection,
		HexCellComponent castCell, 
		HexCellComponent casterStandingCell, 
		Transform casterTransform)
	{
		// Create the blast handler
		GameObject blastHandlerObject = new GameObject("BlastHandler");
		BlastActor blastActor = blastHandlerObject.AddComponent<BlastActor>();
        
		// Initialize the blast
		blastActor.InitBlast(sourceAbility,casterType, objectFx, parameters, castDirection, casterStandingCell);
        
		// Subscribe to OnHit event to apply hit status effects
		blastActor.OnHitApplyStatusEffect += (target) => 
			sourceAbility.ApplyStatusEffects(AbilityStatusApplicationType.OnHit, target);
	}
}