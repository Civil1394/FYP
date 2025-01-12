using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public EnemyData enemyConfig;

    //Functional component
    [SerializeField] protected PlayerDetector playerDetector;
    protected StateMachine stateMachine;
    protected PathFinding pathFinding;

    //Memory
    public Vector3Int currentCoord;
    public HexCell currentCell;
    public HexCellComponent playerGrid;
    public HexCellComponent lastSeenPlayerGrid;
    public HexCellComponent attackTargetGrid;

    //Gizmo
    public List<HexCell> gPath;
    public Color mColor;

    //Stat
    protected IAttack attackStrategy;
    protected int attackDur = 6;
    
    //Control flag
    public bool isAttacking = false;

    protected void Start()
    {
        mColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        playerDetector = GetComponentInChildren<PlayerDetector>();
        pathFinding = new PathFinding();
        stateMachine = new StateMachine();
        
        var wanderState = new GridEnemyWander(this, null, 10, pathFinding);
        var chaseState = new GridEnemyChase(this, null, pathFinding);
        var attackState = new GridEnemyAttack(this, null);
        var retreatState = new GridEnemyRetreat(this, null,pathFinding);
        #region Set up state transition

        stateMachine.AddTransition(
            wanderState, chaseState, new FuncPredicate(
                () => playerDetector.CanDetectPlayer(out playerGrid)
            )
        );
        stateMachine.AddTransition(
            chaseState, wanderState, new FuncPredicate(
                () => !playerDetector.CanDetectPlayer(out playerGrid) && chaseState.HasReachedDestination()
            )
        );
        stateMachine.AddTransition(
            chaseState, attackState, new FuncPredicate(
                () =>
                    playerDetector.CanDetectPlayer(out playerGrid) &&
                    IsPlayerInAttackRange(enemyConfig.AttackRangeInCell) &&
                    attackDur <= 0
            )
        );

        stateMachine.AddTransition(
            attackState, retreatState, new FuncPredicate(
                () => !isAttacking
            )
        );
        stateMachine.AddTransition(
            retreatState, chaseState, new FuncPredicate(
                () => attackDur <= 0
            )
        );
#endregion
        stateMachine.SetState(wanderState);

        InitializeAttackStrategy();
    }

    protected void Update()
    {
        stateMachine.Update();
        RememberPlayer();
    }
    public void TurnAction()
    {
        stateMachine.OnTurnStart();
        attackDur--;
        //print(attackDur);
    }
    protected void InitializeAttackStrategy()
    {
        switch (enemyConfig.AbilityData.CastType)
        {
            case AbilityCastType.Direction_targeted:
                attackStrategy = new DirectionTargeting(transform, enemyConfig.AttackPrefab,enemyConfig.AbilityData);
                break;
            // case EnemyData.AttackType.GroundTargetting:
            //     attackStrategy = new GroundTargeting(transform, enemyConfig.AttackPrefab);
            //     break;
            // case EnemyData.AttackType.Dash:
            //     attackStrategy = new DashAttack(transform, enemyConfig.DashSpeed);
            //     break;
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
        var nextGridPosition = cellToMove.ParentComponent.transform.position;
        Vector3 directionToNextGrid = (nextGridPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToNextGrid);
        transform.DOMove(cellToMove.ParentComponent.transform.position, 0.5f);
        EnemyManager.Instance.OnMove(this, cellToMove.Coordinates);
        transform.DORotateQuaternion(targetRotation, 0.5f);
        currentCoord = cellToMove.Coordinates;
        currentCell = cellToMove;
    }

    public void PerformAttack()
    {
        attackDur = 6;
        print("attacked");
        //Need to use Opposite to reverse the direction from player toward enemy
        //then it can be casting direction
        HexDirection castDirection = HexDirectionExtensions.Opposite(
            BattleManager.Instance.hexgrid.GetHexDirectionBy2Cell(
                playerGrid, currentCell.ParentComponent
            ));
        attackStrategy.Attack(castDirection, currentCell.ParentComponent);

    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = mColor;
        if (gPath == null) return;
        foreach (var p in gPath)
        {
            var temp = BattleManager.Instance.hexgrid.GetCellInCoord(p.Coordinates);
            Gizmos.DrawCube(temp.transform.position, Vector3.one);
        }
    }

    public bool IsPlayerInAttackRange(int range)
    {
        if (!BattleManager.Instance.hexgrid.PlayerSixDirCellsSet.ContainsKey(currentCell.ParentComponent))
        {
            //print("cell dose not exist in 6dir dict");
            return false;
        }
        int tempR = BattleManager.Instance.hexgrid.PlayerSixDirCellsSet[currentCell.ParentComponent];
        if (tempR <= range)
        {
            //print("the distance is "+tempR);
            return true;
        }
        //print("probably out of range");
        return false;
    }
}