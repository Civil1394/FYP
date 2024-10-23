using UnityEngine;

public class DirectionTargeting : IAttack
{
    private Transform enemy;
    private GameObject projectilePrefab;

    public DirectionTargeting(Transform enemy, GameObject projectilePrefab)
    {
        this.enemy = enemy;
        this.projectilePrefab = projectilePrefab;
    }

    public void Attack(HexCellComponent target)
    {
        GameObject projectile = GameObject.Instantiate(projectilePrefab, enemy.position, Quaternion.identity);
        // Add projectile movement logic towards target here
    }
}