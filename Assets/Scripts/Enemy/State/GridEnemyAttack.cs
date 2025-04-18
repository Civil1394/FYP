using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GridEnemyAttack : EnemyBaseState
{
    private AbilityData ad;
    public GridEnemyAttack(AIBrain enemyBrain, Animator animator, AbilityData ad) : base(enemyBrain, animator)
    {
        this.ad = ad;
    }
    public override void OnEnter()
    {
        enemyBrain.isAttacking = true;
        enemyBrain.transform.LookAt(enemyBrain.playerGrid.transform.position);
    }
    public override void OnExit()
    {

    }
    public override void Update()
    {

    }
    public override void FixedUpdate()
    {

    }
    public override void TurnAction()
    {
        switch (ad.abilityType)
        {
            case AbilityType.Projectile:
            case AbilityType.Blast:
            case AbilityType.ProjectileVolley:
                DirectionAttack();
                break;
            case AbilityType.Dash:
            case AbilityType.LocationalProjectile:
                LocationalAttack();
                break;
        }
        if (enemyBrain.isAttacking) enemyBrain.isAttacking = false;
    }

    public void DirectionAttack()
    {
        HexDirection fuzzyDir =
            BattleManager.Instance.hexgrid.GetFuzzyHexDirectionBy2Cell(enemyBrain.currentCell.ParentComponent,
                BattleManager.Instance.PlayerCell);
        HexCellComponent castCell = enemyBrain.currentCell.GetNeighbor(fuzzyDir).ParentComponent;
        Debug.Log(fuzzyDir.ToString());
        ad.TriggerAbility(CasterType.Enemy, castCell, enemyBrain.currentCell.ParentComponent, enemyBrain.gameObject);
    }

    public void LocationalAttack()
    {
        var cellList = BattleManager.Instance.hexgrid.GetCellsInRange(BattleManager.Instance.PlayerCell, 2);
        var randIdx = Random.Range(0, cellList.Length);
        ad.TriggerAbility(CasterType.Enemy, cellList[randIdx], enemyBrain.currentCell.ParentComponent, enemyBrain.gameObject);
    }
}