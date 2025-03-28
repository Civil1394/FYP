using UnityEngine;

public class DirectionTargeting : IAttack
{
    private Transform enemy;
    private GameObject projectilePrefab;
    private AbilityData abilityData;
    private Hourglass hourglass;
    public DirectionTargeting(Transform enemy, GameObject projectilePrefab, AbilityData abilityData,Hourglass hourglass)
    {
        this.enemy = enemy;
        this.projectilePrefab = projectilePrefab;
        this.abilityData = abilityData;
        this.hourglass = hourglass;
    }

    public void Attack(HexCellComponent castCell, HexCellComponent standingCell)
    {
        abilityData.TriggerAbility(CasterType.Enemy, castCell, standingCell,enemy.gameObject,hourglass.TimeType);
    }
}