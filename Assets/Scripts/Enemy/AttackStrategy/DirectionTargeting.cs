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

    public void Attack(HexCellComponent directionCell, HexCellComponent standingCell)
    {
        GameObject projectile = GameObject.Instantiate(projectilePrefab, enemy.position, Quaternion.identity);
        //Debug.Log(directionCell   .CellData.Coordinates);
        Vector3 tempDir = directionCell.transform.position-projectile.transform.position;
        tempDir.Normalize();
        projectile.GetComponent<DirectionProjectileBehaviour>().Init(tempDir);
        
        //TODO: need Implement facing toward player first
        // foreach (var effect in abilityData.Effects)
        // {
        //     effect.ApplyEffect(AbilityCasterType.Enemy,directionCell,standingCell);
        // }
    }
}