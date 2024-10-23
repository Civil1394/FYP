using UnityEngine;

public class DashAttack : IAttack
{
    private Transform enemy;
    private float dashSpeed;

    public DashAttack(Transform enemy, float dashSpeed)
    {
        this.enemy = enemy;
        this.dashSpeed = dashSpeed;
    }

    public void Attack(HexCellComponent target)
    {
        Vector3 direction = (target.transform.position - enemy.position).normalized;
        enemy.position += direction * dashSpeed * Time.deltaTime;
        // Add more sophisticated dash logic here if needed
    }
}