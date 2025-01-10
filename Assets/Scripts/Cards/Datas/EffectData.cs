using System; 
using UnityEngine; 
using System.Collections;
using Unity.Mathematics;


[CreateAssetMenu(fileName = "EffectData", menuName = "Effect/EffectData")]
public class EffectData : ScriptableObject
{
    public EffectType effectType;
    [TextArea(5, 7)]
    public string Desc;

    [ConditionalField("effectType", EffectType.Projectile)]
    [SerializeField] ProjectileParameter projectileParam;

    [ConditionalField("effectType", EffectType.Explosive)]
    [SerializeField] ExplosiveParameter explosiveParam;
    
    [ConditionalField("effectType", EffectType.Dash)]
    [SerializeField] DashParameter dashParam;
    
    [Header("FX")]
    [ConditionalField("effectType", EffectType.Projectile,EffectType.Explosive,EffectType.Dash)]
    [SerializeField] GameObject Object_fx;
    
    public void ApplyEffect(HexDirection castDirection,HexCellComponent casterStandingCell)
    {
        switch (effectType)
        {
            case EffectType.Projectile:
                TriggerProjectile(castDirection,casterStandingCell);
                break;
            case EffectType.Explosive:
                TriggerExplosion();
                break;
            case EffectType.Dash:
                TriggerDash();
                break;
            default:
                Debug.LogWarning("Effect type not implemented.");
                break;
        }
    }
    
    private void TriggerProjectile(HexDirection castingDirection, HexCellComponent casterStandingCell)
    {
        if (projectileParam != null)
        {
            HexCellComponent spawnCell =
                BattleManager.Instance.hexgrid.GetCellByDirection(casterStandingCell, castingDirection);
            
            // Create rotation that faces the player's direction but maintains -90 on Y
            //Quaternion spawnRotation = Quaternion.LookRotation(playerForward) * Quaternion.Euler(0, -90, 0);
            Vector3 height_offset = new Vector3(0, 3, 0);
            GameObject bullet = Instantiate(Object_fx, spawnCell.transform.position + height_offset,quaternion.identity);
            var bulletComponent = bullet.AddComponent<BulletActor>();
            
            bulletComponent.Initialize(
                projectileParam.Damage,
                projectileParam.FlowSpeed,
                spawnCell.CellData.Coordinates,
                castingDirection,
                projectileParam.LifeTime,
                height_offset
            );
        }
        else
        {
            Debug.LogError("Projectile parameter not set!");
        }
        //Debug.Log(Desc);
    }
    

    private void TriggerExplosion()
    {
        if (explosiveParam != null)
        {
            // Implementation for explosion effect using explosionParams
            Debug.Log($"Explosive effect triggered: {Desc} with radius {explosiveParam.radius}");
        }
        else
        {
            Debug.LogError("Explosive parameter not set!");
        }
    }

    private void TriggerDash()
    {
        if (dashParam != null)
        {
            Debug.Log("Dash effect triggered");
        }
        else
        {
            Debug.LogError("Dash parameter not set!");
        }
    }
}
public enum EffectType 
{ 
    Projectile = 0, 
    
    Explosive = 10,
    
    Dash = 20
    // Add more effect types as needed 
}