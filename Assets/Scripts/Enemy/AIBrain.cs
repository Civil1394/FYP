using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Runtime.InteropServices.WindowsRuntime;

public class AIBrain : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform testPlayer;
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
        stateMachine.AddAnyTransition(wanderState,new FuncPredicate(()=> seePlayer()==false));
        stateMachine.AddTransition(wanderState, chaseState, new FuncPredicate(()=> seePlayer()));
        stateMachine.SetState(wanderState);
    }
    private void Update()
    {
        seePlayer();
        stateMachine.Update();
    }
    bool seePlayer()
    {
        if (testIsFirstTime)
        {
            testIsFirstTime = false;
        }
        var range = Vector3.Distance(testPlayer.position, transform.position);
        if (range <5)
        {
            playerPos = testPlayer.position;
            return true;
        }
        lastSeemPlayerPos = testPlayer.position;
        return false;
    }
}

