using UnityEngine;

public class DirectionTargeting : IAttack
{
    private Transform enemy;
    private GameObject projectilePrefab;
    private AbilityData abilityData;
    public DirectionTargeting(Transform enemy, GameObject projectilePrefab, AbilityData abilityData)
    {
        this.enemy = enemy;
        this.projectilePrefab = projectilePrefab;
        this.abilityData = abilityData;
    }

    public void Attack(HexDirection castDirection, HexCellComponent standingCell)
    {
        foreach (var effect in abilityData.Effects)
        {
            effect.ApplyEffectDirection(enemy, castDirection, standingCell);
        }
    }
}