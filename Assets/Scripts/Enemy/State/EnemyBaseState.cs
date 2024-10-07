using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public abstract class EnemyBaseState : IState
{
    protected readonly AIBrain enemyBrain;
    protected readonly Animator animator;
    protected const float crossFadeDuration = 0.1f;

    protected EnemyBaseState(AIBrain enemyBrain, Animator animator)
    {
        this.enemyBrain = enemyBrain;
        this.animator = animator;
    }
    public virtual void OnEnter()
    {

    }
    public virtual void OnExit()
    {

    }
    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }
}

