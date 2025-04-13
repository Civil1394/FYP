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
		DashActor projectileActor = dashObject.AddComponent<DashActor>();
		projectileActor.InitDash(casterType, parameters, castCell, casterTransform.GetComponent<TimedActor>());
        
		// Subscribe to OnHit event to apply hit status effects
		projectileActor.OnHitApplyStatusEffect += (target) => 
			sourceAbility.ApplyStatusEffects(AbilityStatusApplicationType.OnHit, target);
	}
}