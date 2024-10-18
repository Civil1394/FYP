using UnityEngine;
using UnityEngine.AI;

public class AIBrain : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform testPlayer;
    [SerializeField] AISensor visionSenor;
    StateMachine stateMachine;

    public Vector3 alertPos;
    public Vector3 playerPos;
    public Vector3 lastSeemPlayerPos;
    public Vector3 projectileVector;

    bool testIsFirstTime = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new StateMachine();
        var wanderState = new EnemyWander(this, null, agent, 3);
        var chaseState = new EnemyChase(this, null, agent);
        stateMachine.AddAnyTransition(wanderState,new FuncPredicate(()=> !visionSenor.seePlayer));
        stateMachine.AddTransition(wanderState, chaseState, new FuncPredicate(()=> visionSenor.seePlayer));
        stateMachine.SetState(wanderState);
    }
    private void Update()
    {
        stateMachine.Update();
    }
}