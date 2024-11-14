using UnityEngine;

public class GridEnemyAttack : EnemyBaseState
{
    public GridEnemyAttack(AIBrain enemyBrain, Animator animator) : base(enemyBrain, animator)
    {
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
        enemyBrain.PerformAttack();
        if (enemyBrain.isAttacking) enemyBrain.isAttacking = false;
    }
}