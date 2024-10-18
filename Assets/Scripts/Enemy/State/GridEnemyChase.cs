using UnityEngine;

public class GridEnemyChase : EnemyBaseState
{
    public GridEnemyChase(AIBrain enemyBrain, Animator animator) : base(enemyBrain, animator)
    {
    }
    public override void OnEnter()
    {
        Debug.Log("start wandering");
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
}