using UnityEngine;

public class DashAttack : IAttack
{
    private Transform enemy;
    private Transform target;
    private float dashSpeed;

    public DashAttack(Transform enemy, Transform target, float dashSpeed)
    {
        this.enemy = enemy;
        this.target = target;
        this.dashSpeed = dashSpeed;
    }

    public void Attack()
    {
        Vector3 direction = (target.position - enemy.position).normalized;
        enemy.position += direction * dashSpeed * Time.deltaTime;
        // Add more sophisticated dash logic here if needed
    }
}