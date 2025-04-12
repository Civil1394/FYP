using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public EnemyData enemyConfig;

    //Functional component
    [SerializeField] private PlayerDetector playerDetector;
    public EnemyActor enemyActor;
    private StateMachine stateMachine;
    private PathFinding pathFinding;
    private IHexPatternHelper attackPattern;

    //Memory
    public Vector3Int currentCoord;
    public HexCell currentCell;
    public HexCellComponent playerGrid = null;
    public HexCellComponent attackTargetGrid;

    //Gizmo
    public List<HexCell> gPath;
    public Color mColor;

    //Stat
    private IAttack attackStrategy;
    private int attackDur = 2;
    
    //Control flag
    public bool isAttacking = false;

    //State
    private GridEnemyWander wanderState;
    private GridEnemyChase chaseState;
    private GridEnemyAttack attackState;
    private GridEnemyRetreat retreatState;
    private void Start()
    {
        mColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        playerDetector = GetComponent<PlayerDetector>();
        playerDetector.Init(this, enemyConfig.AngleOfRange, enemyConfig.DistanceOfRange, enemyConfig.InnerSphereRadius);
        pathFinding = new PathFinding();
        stateMachine = new StateMachine();
        StateInitialization();
        stateMachine.SetState(wanderState);

        InitializeAttackStrategy();
    }

    void StateInitialization()
    {
        switch (enemyConfig.EnemyStateType)
        {
            case EnemyData.EnemyType.Berserk:
                BerserkState();
                break;
            case EnemyData.EnemyType.Sniper:
                SniperState();
                break;
        }
    }

    void BerserkState()
    {
        wanderState = new GridEnemyWander(this, null, 10, pathFinding);
        chaseState = new GridEnemyChase(this, null, pathFinding);
        attackState = new GridEnemyAttack(this, null, enemyConfig.AbilityData);
        #region Set up state transition

        stateMachine.AddTransition(
            wanderState, chaseState, new FuncPredicate(
                //() => playerDetector.CanDetectPlayer(out playerGrid)
                () => { return (bool)playerGrid; }
            )
        );
        stateMachine.AddTransition(
            chaseState, attackState, new FuncPredicate(
                () =>
                    (bool)playerGrid &&
                    IsPlayerInAttackRange(enemyConfig.AttackRangeInCell) &&
                    attackDur <= 0
            )
        );

        stateMachine.AddTransition(
            attackState, chaseState, new FuncPredicate(
                () => !isAttacking
            )
        );
        #endregion
    }
    void SniperState()
    {
        wanderState = new GridEnemyWander(this, null, 10, pathFinding);
        chaseState = new GridEnemyChase(this, null, pathFinding);
        attackState = new GridEnemyAttack(this, null, enemyConfig.AbilityData);
        retreatState = new GridEnemyRetreat(this, null, pathFinding, 10);
        #region Set up state transition

        stateMachine.AddTransition(
            wanderState, chaseState, new FuncPredicate(
                () => (bool)playerGrid
            )
        );

        stateMachine.AddTransition(
            chaseState, attackState, new FuncPredicate(
                () =>
                    IsPlayerInAttackRange(enemyConfig.AttackRangeInCell) &&
                    attackDur <= 0
            )
        );

        stateMachine.AddTransition(
            attackState, chaseState, new FuncPredicate(
                () => !isAttacking
            )
        );
        stateMachine.AddAnyTransition(retreatState, new FuncPredicate(
                () =>
                    (bool)playerGrid &&
                    BattleManager.Instance.hexgrid.GetGridDistance(currentCell.ParentComponent, playerGrid) < 5
            )
        );
        stateMachine.AddTransition(
            retreatState, chaseState, new FuncPredicate(
                () =>
                    (bool)playerGrid &&
                    BattleManager.Instance.hexgrid.GetGridDistance(currentCell.ParentComponent, playerGrid) >= 5
            )
        );
        #endregion
    }

    void BoomerState()
    {
        wanderState = new GridEnemyWander(this, null, 10, pathFinding);
        chaseState = new GridEnemyChase(this, null, pathFinding);
        attackState = new GridEnemyAttack(this, null, enemyConfig.AbilityData);
        #region Set up state transition

        stateMachine.AddTransition(
            wanderState, chaseState, new FuncPredicate(
                () => (bool)playerGrid
            )
        );
        stateMachine.AddTransition(
            chaseState, attackState, new FuncPredicate(
                () =>
                    IsPlayerInAttackRange(enemyConfig.AttackRangeInCell) &&
                    attackDur <= 0
            )
        );

        stateMachine.AddTransition(
            attackState, chaseState, new FuncPredicate(
                () => !isAttacking
            )
        );
        #endregion
    }

    void AssassinState()
    {
        wanderState = new GridEnemyWander(this, null, 10, pathFinding);
        chaseState = new GridEnemyChase(this, null, pathFinding);
        attackState = new GridEnemyAttack(this, null, enemyConfig.AbilityData);
        retreatState = new GridEnemyRetreat(this, null, pathFinding, 3);
        #region Set up state transition

        stateMachine.AddTransition(
            wanderState, chaseState, new FuncPredicate(
                () => (bool)playerGrid
            )
        );
        stateMachine.AddTransition(
            chaseState, attackState, new FuncPredicate(
                () =>
                    IsPlayerInAttackRange(enemyConfig.AttackRangeInCell) &&
                    attackDur <= 0
            )
        );

        stateMachine.AddTransition(
            attackState, chaseState, new FuncPredicate(
                () => !isAttacking
            )
        );
        stateMachine.AddAnyTransition(retreatState, new FuncPredicate(
                () =>
                    (bool)playerGrid &&
                    BattleManager.Instance.hexgrid.GetGridDistance(currentCell.ParentComponent, playerGrid) < 5
            )
        );
        stateMachine.AddTransition(
            retreatState, chaseState, new FuncPredicate(
                () =>
                    (bool)playerGrid &&
                    BattleManager.Instance.hexgrid.GetGridDistance(currentCell.ParentComponent, playerGrid) >= 5
            )
        );
        #endregion
    }
    private void Update()
    {
        stateMachine.Update();
    }
    public void TurnAction()
    {
        stateMachine.OnTurnStart();
        attackDur--;
        //print(attackDur);
    }
    private void InitializeAttackStrategy()
    {
        switch (enemyConfig.AbilityData.CastType)
        {
            case AbilityCastType.Direction_targeted:
                attackStrategy = new DirectionTargeting(transform, enemyConfig.AttackPrefab,enemyConfig.AbilityData,enemyActor.hourglass);
                break;
            // case EnemyData.AttackType.GroundTargetting:
            //     attackStrategy = new GroundTargeting(transform, enemyConfig.AttackPrefab);
            //     break;
            // case EnemyData.AttackType.Dash:
            //     attackStrategy = new DashAttack(transform, enemyConfig.DashSpeed);
            //     break;
        }
    }
    public void Move(HexCell cellToMove)
    {
        EnemyManager.Instance.ReleaseCell(this);
        var nextGridPosition = cellToMove.ParentComponent.transform.position;
        Vector3 directionToNextGrid = (nextGridPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToNextGrid);
        transform.DOMove(cellToMove.ParentComponent.transform.position, enemyActor.ActionCooldown);
        EnemyManager.Instance.OnMove(this, cellToMove.Coordinates);
        transform.DORotateQuaternion(targetRotation, enemyActor.ActionCooldown);
        currentCoord = cellToMove.Coordinates;
        currentCell = cellToMove;
    }

    public void PerformAttack()
    {
        //this attack need to rework by using the abilty data, the chase also need to rework
        attackDur = 6;
        print("attacked");
        //Need to use Opposite to reverse the direction from player toward enemy
        //then it can be casting direction
        // HexDirection castDirection = HexDirectionHelper.Opposite(
        //     BattleManager.Instance.hexgrid.GetHexDirectionBy2Cell(
        //         playerGrid, currentCell.ParentComponent
        //     ));
        // attackStrategy.Attack(castDirection, currentCell.ParentComponent);
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

    public bool IsPlayerInAttackRange(int range)
    {
        //Bug attack without direct line of sight probably
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