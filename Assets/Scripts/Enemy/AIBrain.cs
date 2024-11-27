using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    [SerializeField] EnemyData enemyConfig;

    //Functional component
    [SerializeField] PlayerDetector playerDetector;
    StateMachine stateMachine;
    PathFinding pathFinding;

    //Memory
    public Vector3Int currentCoord;
    public HexCellComponent playerGrid;
    public HexCellComponent lastSeenPlayerGrid;
    public HexCellComponent attackTargetGrid;

    //Gizmo
    public List<HexCell> gPath;
    public Color mColor;

    //Stat
    private IAttack attackStrategy;
    private int attackDur = 6;

    //Control flag
    public bool isAttacking = false;

    private void Start()
    {
        mColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        playerDetector = GetComponentInChildren<PlayerDetector>();
        pathFinding = new PathFinding();
        stateMachine = new StateMachine();

        BattleManager.Instance.OnTurnStart.AddListener(TurnAction);

        var wanderState = new GridEnemyWander(this, null, 10, pathFinding);
        var chaseState = new GridEnemyChase(this, null, pathFinding);
        var attackState = new GridEnemyAttack(this, null);
        #region Set up state transition
        stateMachine.AddTransition(
            chaseState, wanderState, new FuncPredicate(
                () => !playerDetector.CanDetectPlayer(out playerGrid) && chaseState.HasReachedDestination()
                )
            );
        stateMachine.AddTransition(
            wanderState, chaseState, new FuncPredicate(
                () => playerDetector.CanDetectPlayer(out playerGrid)
                )
            );

        stateMachine.AddTransition(
            chaseState, attackState, new FuncPredicate(
                () =>
                playerDetector.CanDetectPlayer(out playerGrid) &&
                Vector3.Distance(transform.position, playerGrid.transform.position) < 20 &&
                attackDur <= 0
                )
            );

        stateMachine.AddTransition(
            attackState, chaseState, new FuncPredicate(
                () => !isAttacking
                )
            );
#endregion
        stateMachine.SetState(wanderState);

        InitializeAttackStrategy();
    }

    private void Update()
    {
        stateMachine.Update();
    }
    private void TurnAction()
    {
        stateMachine.OnTurnStart();
        RememberPlayer();
        attackDur--;
        //print(attackDur);
    }
    private void InitializeAttackStrategy()
    {
        switch (enemyConfig.AttackStrategy)
        {
            case EnemyData.AttackType.DirectionTargetting:
                attackStrategy = new DirectionTargeting(transform, enemyConfig.AttackPrefab);
                break;
            case EnemyData.AttackType.GroundTargetting:
                attackStrategy = new GroundTargeting(transform, enemyConfig.AttackPrefab);
                break;
            case EnemyData.AttackType.Dash:
                attackStrategy = new DashAttack(transform, enemyConfig.DashSpeed);
                break;
        }
    }
    public void RememberPlayer()
    {
        if(playerGrid)
            lastSeenPlayerGrid = playerGrid;
    }
    public void Move(HexCell cellToMove)
    {
        EnemyManager.Instance.ReleaseCell(this);
        var nextGridPosition = BattleManager.Instance.hexgrid.GetCellInCoord(cellToMove.Coordinates).transform.position;
        Vector3 directionToNextGrid = (nextGridPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToNextGrid);
        transform.DOMove(BattleManager.Instance.hexgrid.GetCellInCoord(cellToMove.Coordinates).transform.position, 0.5f);
        EnemyManager.Instance.OnMove(this, cellToMove.Coordinates);
        transform.DORotateQuaternion(targetRotation, 0.5f);
        currentCoord = cellToMove.Coordinates;
    }

    public void PerformAttack()
    {
        attackDur = 6;
        attackStrategy.Attack(playerGrid);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = mColor;
        if (gPath == null) return;
        foreach (var p in gPath)
        {
            var temp = BattleManager.Instance.hexgrid.GetCellInCoord(p.Coordinates);
            Gizmos.DrawCube(temp.transform.position, Vector3.one);
        }
    }
}
