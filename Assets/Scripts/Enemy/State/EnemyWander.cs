using UnityEngine;
using UnityEngine.AI;

public class EnemyWander : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Vector3 startPoint;
    readonly float wanderRadius;

    public EnemyWander(AIBrain enemyBrain, Animator animator, NavMeshAgent agent, float wanderRadius):base(enemyBrain, animator)
    {
        this.agent = agent;
        startPoint = enemyBrain.transform.position;
        this.wanderRadius = wanderRadius;
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
        if (HasReachedDestination())
        {
            var randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startPoint;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
            var finalPosition = hit.position;

            agent.SetDestination(finalPosition);
        }
    }

    public override void FixedUpdate()
    {

    }
    bool HasReachedDestination()
    {
        return !agent.pathPending
               && agent.remainingDistance <= agent.stoppingDistance
               && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }
}