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
        Debug.Log(target.CellData.Coordinates);
        //projectile.transform.DOMove(target.transform.position, 1);
        Vector3 tempDir = target.transform.position-projectile.transform.position;
        tempDir.Normalize();
        projectile.GetComponent<DirectionProjectileBehaviour>().Init(tempDir);

        // Add projectile movement logic towards target here
    }
}