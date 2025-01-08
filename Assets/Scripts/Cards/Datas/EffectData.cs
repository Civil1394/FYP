using System; 
using UnityEngine; 
using System.Collections;
using Unity.Mathematics;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "EffectData", menuName = "Effect/EffectData")]
public class EffectData : ScriptableObject
{
    [SerializeField] private EffectType effectType;
    [TextArea(5, 7)]
    public string Desc;

    [Header("FX")]
    public GameObject Object_fx;

    // This will show different parameter objects based on the effect type
    [SerializeField] private EffectParameters parameters;

    public void ApplyEffect(HexDirection castDirection,HexCellComponent casterStandingCell)
    {
        switch (effectType)
        {
            case EffectType.Projectile:
                TriggerProjectile(castDirection,casterStandingCell);
                break;
            case EffectType.Explosion:
                TriggerExplosion();
                break;
            default:
                Debug.LogWarning("Effect type not implemented.");
                break;
        }
    }
    
    private void TriggerProjectile(HexDirection castingDirection, HexCellComponent casterStandingCell)
    {
        if (parameters is ProjectileParameters projectileParams)
        {
            HexCellComponent spawnCell =
                BattleManager.Instance.hexgrid.GetCellByDirection(casterStandingCell, castingDirection);
            
            // Create rotation that faces the player's direction but maintains -90 on Y
            //Quaternion spawnRotation = Quaternion.LookRotation(playerForward) * Quaternion.Euler(0, -90, 0);
            Vector3 height_offset = new Vector3(0, 3, 0);
            GameObject bullet = Instantiate(Object_fx, spawnCell.transform.position + height_offset,quaternion.identity);
            var bulletComponent = bullet.AddComponent<BulletActor>();
            
            bulletComponent.Initialize(
                projectileParams.Damage,
                projectileParams.FlowSpeed,
                spawnCell.CellData.Coordinates,
                castingDirection,
                projectileParams.LifeTime,
                height_offset
            );
        }
        else
        {
            Debug.LogError("Projectile parameters not set!");
        }
        //Debug.Log(Desc);
    }
    

    private void TriggerExplosion()
    {
        if (parameters is ExplosionParameters explosionParams)
        {
            // Implementation for explosion effect using explosionParams
            Debug.Log($"Explosion effect triggered: {Desc} with radius {explosionParams.radius}");
        }
        else
        {
            Debug.LogError("Explosion parameters not set!");
        }
    }
}
public enum EffectType 
{ 
    Projectile = 0, 
    Explosion = 1 
    // Add more effect types as needed 
}