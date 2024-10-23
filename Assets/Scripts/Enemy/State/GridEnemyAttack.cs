using UnityEngine;

public class GridEnemyAttack : EnemyBaseState
{
    public GridEnemyAttack(AIBrain enemyBrain, Animator animator) : base(enemyBrain, animator)
    {
    }
    public override void OnEnter()
    {
        enemyBrain.PerformAttack();
        Debug.Log("Attacked");
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
        if (enemyBrain.isAttacking) return;
    }
}