using UnityEngine;

public class GroundTargeting : IAttack
{
    private Transform enemy;
    private GameObject bombPrefab;
    private Transform target;

    public GroundTargeting(Transform enemy, GameObject bombPrefab, Transform target)
    {
        this.enemy = enemy;
        this.bombPrefab = bombPrefab;
        this.target = target;
    }

    public void Attack()
    {
        GameObject bomb = GameObject.Instantiate(bombPrefab, enemy.position, Quaternion.identity);
        // Add bomb throwing logic towards target here
    }
}