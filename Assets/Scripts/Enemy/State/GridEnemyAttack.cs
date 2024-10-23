using UnityEngine;

public class GridEnemyAttack : EnemyBaseState
{
    private bool isAttacking;
    public GridEnemyAttack(AIBrain enemyBrain, Animator animator) : base(enemyBrain, animator)
    {
    }
    public override void OnEnter()
    {
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
        if (isAttacking) return;
    }
}