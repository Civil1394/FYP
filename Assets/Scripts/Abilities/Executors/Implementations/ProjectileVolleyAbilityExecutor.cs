using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

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
		FireVolleyAsync(casterType, castDirection, casterStandingCell).Forget();
	}
	
	private async UniTask FireVolleyAsync(CasterType casterType, HexDirection castDirection, HexCellComponent casterStandingCell)
	{
		for (int i = 0; i < parameters.BurstCount; i++)
		{
			await Burst(casterType, castDirection, casterStandingCell);
        
			// Wait for the delay between bursts (except after the last shot)
			if (i < parameters.BurstCount - 1)
			{
				await UniTask.Delay((int)(parameters.DelayBetweenBurst * 1000)); // Convert seconds to milliseconds
			}
		}
	}

	private async UniTask Burst(CasterType casterType, HexDirection castDirection, HexCellComponent casterStandingCell)
	{
		for (int i = 0; i < parameters.ProjectilePerBurst; i++)
		{
			Vector3 randDelta = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			HexCellComponent spawnCell =
				BattleManager.Instance.hexgrid.GetCellByDirection(casterStandingCell, castDirection);
			GameObject bulletObject = Object.Instantiate(objectFx,
				spawnCell.transform.position + parameters.ProjectileConfig.VFX_Height_Offset + randDelta, Quaternion.identity);
			var bulletComponent = bulletObject.AddComponent<ProjectileActor>();
			bulletComponent.InitBullet(
				casterType,
				parameters.ProjectileConfig,
				castDirection,
				spawnCell
			);

			if (i < parameters.ProjectilePerBurst - 1)
			{
				await UniTask.Delay((int)(parameters.DelayBetweenProjectiles * 1000));
			}
		}
		
	}
}