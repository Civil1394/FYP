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
        var wanderState = new GridEnemyWander(this, null, 10);
        stateMachine.AddAnyTransition(wanderState, new FuncPredicate(() => true));
        //var chaseState = new EnemyChase(this, null, agent);
        //stateMachine.AddTransition(chaseState, wanderState, new FuncPredicate(()=> !playerDetector.CanDetectPlayer(out player)));
        //stateMachine.AddTransition(wanderState, chaseState, new FuncPredicate(()=> playerDetector.CanDetectPlayer(out player)));
        stateMachine.SetState(wanderState);
        StartCoroutine(TestTurn());
    }
    private void Update()
    {
        stateMachine.Update();
    }
    private void RememberPlayer()
    {
        if (player != null) lastSeemPlayer = player;
    }
    public void Move(HexCell cellToMove)
    {
        transform.position = BattleManager.Instance.hexgrid.GetCell(cellToMove.Coordinates).transform.position;
        currentCoord = cellToMove.Coordinates;
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