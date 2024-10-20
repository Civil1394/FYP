using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Unity.Mathematics;
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
        var nextGridPosition = BattleManager.Instance.hexgrid.GetCell(cellToMove.Coordinates).transform.position;
        Vector3 directionToNextGrid = (nextGridPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToNextGrid);

        // Use DOTween to rotate towards the target direction

        transform.DOMove(BattleManager.Instance.hexgrid.GetCell(cellToMove.Coordinates).transform.position, 0.5f);
        transform.DORotateQuaternion(targetRotation, 0.5f);
        currentCoord = cellToMove.Coordinates;
    }
    IEnumerator TestTurn()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            RememberPlayer();
            stateMachine.OnTurnStart();
        }
    }
}