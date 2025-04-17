using UnityEngine;
using System.Collections;

public class LocationalProjectileAbilityExecuter : AbilityExecutorBase 
{
	private readonly LocationalProjectileParameter parameters;
    
	public LocationalProjectileAbilityExecuter(AbilityData sourceAbility) : base(sourceAbility)
	{
		this.parameters = sourceAbility.locationalProjectileParam;
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
		LocationalProjectileActor projectileActor = bulletObject.AddComponent<LocationalProjectileActor>();
		projectileActor.InitBullet(sourceAbility,casterType, parameters, casterStandingCell, castCell);
        
		// Subscribe to OnHit event to apply hit status effects
		projectileActor.OnHitApplyStatusEffect += (target) => 
			sourceAbility.ApplyStatusEffects(AbilityStatusApplicationType.OnHit, target);
	}
}