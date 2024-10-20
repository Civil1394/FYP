using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIBrain : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform testPlayer;
    [SerializeField] PlayerDetector playerDetector;
    StateMachine stateMachine;

    public Vector3Int currentCoord;
    public Vector3 alertPos;
    public Transform player;
    public Transform lastSeemPlayer;
    public Vector3 projectileVector;

    private void Start()
    {
        playerDetector = GetComponent<PlayerDetector>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new StateMachine();
        var wanderState = new EnemyWander(this, null, agent, 3);
        var chaseState = new EnemyChase(this, null, agent);
        stateMachine.AddTransition(chaseState, wanderState, new FuncPredicate(()=> !playerDetector.CanDetectPlayer(out player)));
        stateMachine.AddTransition(wanderState, chaseState, new FuncPredicate(()=> playerDetector.CanDetectPlayer(out player)));
        stateMachine.SetState(wanderState);
    }
    private void Update()
    {

    }
    private void RememberPlayer()
    {
        if (player != null) lastSeemPlayer = player;
    }
    public void Move(HexCell cellToMove)
    {
        transform.position = BattleManager.Instance.hexgrid.GetCell(cellToMove.Coordinates).transform.position;
    }
    IEnumerator TestTurn()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            RememberPlayer();
            stateMachine.OnTurnStart();
        }
    }
}