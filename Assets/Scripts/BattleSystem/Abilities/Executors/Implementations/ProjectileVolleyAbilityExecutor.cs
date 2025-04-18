using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

public class ProjectileVolleyAbilityExecutor : AbilityExecutorBase
{
    private readonly ProjectileVolleyParameter parameters;
    
    public ProjectileVolleyAbilityExecutor(AbilityData sourceAbility) : base(sourceAbility)
    {
        this.parameters = sourceAbility.projectileVolleyParam;
    }
    
    protected override void ExecuteAbilitySpecific(
        CasterType casterType, 
        HexDirection castDirection,
        HexCellComponent castCell, 
        HexCellComponent casterStandingCell, 
        Transform casterTransform)
    {
        FireVolleyAsync(casterType, castDirection, castCell,casterTransform).Forget();
    }
    
    private async UniTask FireVolleyAsync(CasterType casterType, HexDirection castDirection, HexCellComponent castCell,Transform casterTransform)
    {
        for (int i = 0; i < parameters.BurstCount; i++)
        {
            await Burst(casterType, castDirection, castCell,casterTransform);
            
            // Wait for the delay between bursts (except after the last shot)
            if (i < parameters.BurstCount - 1)
            {
                await UniTask.Delay((int)(parameters.DelayBetweenBurst * 1000));
            }
        }
    }
    
    private async UniTask Burst(CasterType casterType, HexDirection castDirection, HexCellComponent castCell,Transform casterTransform)
    {
        for (int i = 0; i < parameters.ProjectilePerBurst; i++)
        {
            Vector3 randDelta = new Vector3(
                UnityEngine.Random.Range(-1f, 1f), 
                UnityEngine.Random.Range(-1f, 1f), 
                UnityEngine.Random.Range(-1f, 1f));
                
            HexCellComponent spawnCell = BattleManager.Instance.hexgrid.GetCellByDirection(castCell, castDirection);
            HexCellComponent randomNeighbor = spawnCell.CellData.Neighbors[UnityEngine.Random.Range(0, spawnCell.CellData.Neighbors.Length)].ParentComponent;
            GameObject projectileObject = UnityEngine.Object.Instantiate(
                objectFx,
                casterTransform.position + parameters.ProjectileConfig.VFX_Height_Offset + randDelta, 
                Quaternion.identity);
                
            ProjectileActor projectileComponent = projectileObject.AddComponent<ProjectileActor>();
            projectileComponent.InitBullet(sourceAbility, casterType, parameters.ProjectileConfig, castDirection, randomNeighbor, casterTransform);
            
            // Subscribe to OnHit event to apply hit status effects
            projectileComponent.OnHitApplyStatusEffect += (target) => 
                sourceAbility.ApplyStatusEffects(AbilityStatusApplicationType.OnHit, target);
            
            if (i < parameters.ProjectilePerBurst - 1)
            {
                await UniTask.Delay((int)(parameters.DelayBetweenProjectiles * 1000));
            }
        }
    }
}