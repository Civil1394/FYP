using DG.Tweening;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    [SerializeField] EnemyData enemyConfig;
    [SerializeField] PlayerDetector playerDetector;
    StateMachine stateMachine;
    public Vector3Int currentCoord;
    public HexCellComponent playerGrid;
    public HexCellComponent lastSeenPlayerGrid;
    public HexCellComponent attackTargetGrid;
    public List<HexCell> gPath;
    public Color mColor;
    private IAttack attackStrategy;
    public int attackDur = 3;
    public bool isAttacking = false;
    private void Start()
    {
        mColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        playerDetector = GetComponentInChildren<PlayerDetector>();

        stateMachine = new StateMachine();
        BattleManager.Instance.OnTurnStart.AddListener(TurnAction);

        var wanderState = new GridEnemyWander(this, null, 10);
        var chaseState = new GridEnemyChase(this, null);
        var attackState = new GridEnemyAttack(this, null);
        stateMachine.AddTransition(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer(out playerGrid)));
        stateMachine.AddTransition(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer(out playerGrid)));
        stateMachine.AddTransition(chaseState, attackState, new FuncPredicate(() => Vector3.Distance(transform.position, playerGrid.transform.position)<20&&attackDur<=0));
        stateMachine.AddTransition(attackState, chaseState, new FuncPredicate(() => !isAttacking));
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
        attackDur = 3;
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
