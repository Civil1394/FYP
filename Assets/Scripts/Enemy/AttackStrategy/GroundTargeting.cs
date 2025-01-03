using UnityEngine;

public class GroundTargeting : IAttack
{
    private Transform enemy;
    private GameObject bombPrefab;

    public GroundTargeting(Transform enemy, GameObject bombPrefab)
    {
        this.enemy = enemy;
        this.bombPrefab = bombPrefab;
    }
    

    public void Attack(HexCellComponent target, HexCellComponent standingCell)
    {
        GameObject bomb = GameObject.Instantiate(bombPrefab, enemy.position, Quaternion.identity);
    }
}