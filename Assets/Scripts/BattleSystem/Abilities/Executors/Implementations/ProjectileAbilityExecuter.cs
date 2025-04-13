using UnityEngine;
using System.Collections;

public class ProjectileAbilityExecutor : AbilityExecutorBase
{
	private readonly ProjectileParameter parameters;
    
	public ProjectileAbilityExecutor(AbilityData sourceAbility) : base(sourceAbility)
	{
		this.parameters = sourceAbility.projectileParam;
	}
    
	protected override void ExecuteAbilitySpecific(
		CasterType casterType, 
		HexDirection castDirection,
		HexCellComponent castCell, 
		HexCellComponent casterStandingCell, 
		Transform casterTransform)
	{
		// Instantiate projectile
		GameObject bulletObject = UnityEngine.Object.Instantiate(
			objectFx, 
			casterTransform.position + parameters.VFX_Height_Offset, 
			Quaternion.identity);
            
		// Initialize projectile
		ProjectileActor projectileActor = bulletObject.AddComponent<ProjectileActor>();
		projectileActor.InitBullet(casterType, parameters, castDirection, castCell, casterTransform);
        
		// Subscribe to OnHit event to apply hit status effects
		projectileActor.OnHitApplyStatusEffect += (target) => 
			sourceAbility.ApplyStatusEffects(AbilityStatusApplicationType.OnHit, target);
	}
}