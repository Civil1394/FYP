using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;


public class AIBrain : MonoBehaviour
{

    [SerializeField] PlayerDetector playerDetector;
    StateMachine stateMachine;

    public Vector3Int currentCoord;
    public HexCellComponent playerGrid;
    public HexCellComponent lastSeenPlayerGrid;

    private void Start()
    {
        playerDetector = GetComponentInChildren<PlayerDetector>();
        //agent = GetComponent<NavMeshAgent>();
        stateMachine = new StateMachine();
        var wanderState = new GridEnemyWander(this, null, 10);
        var chaseState = new GridEnemyChase(this, null);
        stateMachine.AddTransition(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer(out playerGrid)));
        stateMachine.AddTransition(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer(out playerGrid)));
        stateMachine.SetState(wanderState);
        StartCoroutine(TestTurn());
    }
    private void Update()
    {
        stateMachine.Update();
    }
    private void RememberPlayer()
    {
        if (playerGrid != null) lastSeenPlayerGrid = playerGrid;
    }
    public void Move(HexCell cellToMove)
    {
        var nextGridPosition = BattleManager.Instance.hexgrid.GetCellInCoord(cellToMove.Coordinates).transform.position;
        Vector3 directionToNextGrid = (nextGridPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToNextGrid);

        // Use DOTween to rotate towards the target direction

        transform.DOMove(BattleManager.Instance.hexgrid.GetCellInCoord(cellToMove.Coordinates).transform.position, 0.5f);
        EnemyManager.Instance.OnMove(this, cellToMove.Coordinates);
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