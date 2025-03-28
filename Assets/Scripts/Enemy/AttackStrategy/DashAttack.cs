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
    
    

    public void Attack(HexCellComponent castCell, HexCellComponent standingCell)
    {
        // Vector3 direction = (castDirection.transform.position - enemy.position).normalized;
        // enemy.position += direction * dashSpeed * Time.deltaTime;
    }
}