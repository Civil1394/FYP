using UnityEngine;
using System.Collections;

public class DashAbilityExecutor : AbilityExecutorBase 
{
	private readonly DashParameter parameters;
    
	public DashAbilityExecutor(AbilityData sourceAbility) : base(sourceAbility)
	{
		this.parameters = sourceAbility.dashParam;
	}
    
	protected override void ExecuteAbilitySpecific(
		CasterType casterType, 
		HexDirection castDirection,
		HexCellComponent castCell, 
		HexCellComponent casterStandingCell, 
		Transform casterTransform)
	{
		// Instantiate projectile
		GameObject dashObject = UnityEngine.Object.Instantiate(
			objectFx, 
			casterTransform.position + parameters.VFX_Height_Offset, 
			Quaternion.identity);
            
		// Initialize projectile
		DashActor dashActor = dashObject.AddComponent<DashActor>();
		dashActor.InitDash(sourceAbility,casterType, parameters, castCell, casterTransform.GetComponent<TimedActor>());
        
		// Subscribe to OnHit event to apply hit status effects
		dashActor.OnHitApplyStatusEffect += (target) => 
			sourceAbility.ApplyStatusEffects(AbilityStatusApplicationType.OnHit, target);
	}
}