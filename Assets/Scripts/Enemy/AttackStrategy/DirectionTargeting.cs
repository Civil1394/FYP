using UnityEngine;

public class DirectionTargeting : IAttack
{
    private Transform enemy;
    private GameObject projectilePrefab;
    private Transform target;

    public DirectionTargeting(Transform enemy, GameObject projectilePrefab, Transform target)
    {
        this.enemy = enemy;
        this.projectilePrefab = projectilePrefab;
        this.target = target;
    }

    public void Attack()
    {
        GameObject projectile = GameObject.Instantiate(projectilePrefab, enemy.position, Quaternion.identity);
        // Add projectile movement logic towards target here
    }
}