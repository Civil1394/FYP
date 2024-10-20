using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyChase : EnemyBaseState 
{
    readonly NavMeshAgent agent;
    public EnemyChase(AIBrain enemyBrain, Animator animator, NavMeshAgent agent) : base(enemyBrain, animator)
    {
        this.agent = agent;
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
        NavMeshHit hit;
        Vector3 targetPos = enemyBrain.player? enemyBrain.player.position : enemyBrain.lastSeenPlayer.position;
        NavMesh.SamplePosition(targetPos, out hit, 1.0f, NavMesh.AllAreas);
        var finalPosition = hit.position;
        agent.SetDestination(finalPosition);
    }

    public override void FixedUpdate()
    {

    }
}