using System; 
using UnityEngine; 
using System.Collections;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "EffectData", menuName = "Effect/EffectData")]
public class EffectData : ScriptableObject
{
    [SerializeField] private EffectType effectType;
    [TextArea(5, 7)]
    public string desc;

    [Header("FX")]
    public GameObject Object_fx;

    // This will show different parameter objects based on the effect type
    [SerializeField] private EffectParameters parameters;

    public void ApplyEffect(Vector3Int direction)
    {
        switch (effectType)
        {
            case EffectType.Projectile:
                TriggerProjectile(direction);
                break;
            case EffectType.Explosion:
                TriggerExplosion();
                break;
            default:
                Debug.LogWarning("Effect type not implemented.");
                break;
        }
    }
    
    private void TriggerProjectile(Vector3 direction)
    {
        if (parameters is ProjectileParameters projectileParams)
        {
            // Get the player's forward direction
            Vector3 playerForward = BattleManager.Instance.playerInstance.transform.forward.normalized;
        
            // Get spawn position
            Vector3 spawnPosition = BattleManager.Instance.GetPlayerCell().transform.position;
        
            // Create rotation that faces the player's direction but maintains -90 on Y
            Quaternion spawnRotation = Quaternion.LookRotation(playerForward) * Quaternion.Euler(0, -90, 0);
        
            GameObject bullet = Instantiate(Object_fx, spawnPosition, spawnRotation);
            var bulletComponent = bullet.AddComponent<Bullet>();
            bulletComponent.Initialize(
                projectileParams.damage,
                projectileParams.speed,
                playerForward, // Use player's forward direction instead of passed direction
                projectileParams.lifeTime
            );
            bulletComponent.Launch();
        }
        else
        {
            Debug.LogError("Projectile parameters not set!");
        }
        Debug.Log(desc);
    }

    private void TriggerExplosion()
    {
        if (parameters is ExplosionParameters explosionParams)
        {
            // Implementation for explosion effect using explosionParams
            Debug.Log($"Explosion effect triggered: {desc} with radius {explosionParams.radius}");
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