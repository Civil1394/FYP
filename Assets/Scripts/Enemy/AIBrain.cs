using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public EnemyData enemyConfig;

    //Functional component
    [SerializeField] private PlayerDetector playerDetector;
    public EnemyActor enemyActor;
    protected StateMachine stateMachine;
    protected PathFinding pathFinding;
    protected IHexPatternHelper attackPattern;

    //Memory
    public Vector3Int currentCoord;
    public HexCell currentCell;
    public HexCellComponent playerGrid = null;
    public HexCellComponent attackTargetGrid;

    //Gizmo
    public List<HexCell> gPath;
    public Color mColor;

    //Stat
    //protected IAttack attackStrategy;
    protected int attackDur = 2;
    
    //Control flag
    public bool isAttacking = false;

    //State
    private GridEnemyWander wanderState;
    private GridEnemyChase chaseState;
    private GridEnemyAttack attackState;
    private GridEnemyRetreat retreatState;
    
    private AbilityData enemyAbility;
    private void Start()
    {
        mColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        playerDetector.Init(this, enemyConfig.AngleOfRange, enemyConfig.DistanceOfRange, enemyConfig.InnerSphereRadius);
        pathFinding = new PathFinding();
        stateMachine = new StateMachine();
        StateInitialization();
    }

    public virtual void  StateInitialization()
    {
        enemyAbility = enemyConfig.AbilityData.Create(enemyConfig.AbilityData, true);
        switch (enemyConfig.EnemyStateType)
        {
            case EnemyData.EnemyType.Berserk:
                BerserkState(enemyAbility);
                break;
            case EnemyData.EnemyType.Sniper:
                SniperState(enemyAbility);
                break;
        }
        stateMachine.SetState(wanderState);
    }

    void BerserkState(AbilityData ability)
    {
        wanderState = new GridEnemyWander(this, null, 10, pathFinding);
        chaseState = new GridEnemyChase(this, null, pathFinding);
        attackState = new GridEnemyAttack(this, null, ability);
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
    void SniperState(AbilityData ability)
    {
        wanderState = new GridEnemyWander(this, null, 10, pathFinding);
        chaseState = new GridEnemyChase(this, null, pathFinding);
        attackState = new GridEnemyAttack(this, null, ability);
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

    void BoomerState(AbilityData ability)
    {
        wanderState = new GridEnemyWander(this, null, 10, pathFinding);
        chaseState = new GridEnemyChase(this, null, pathFinding);
        attackState = new GridEnemyAttack(this, null, ability);
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

    void AssassinState(AbilityData ability)
    {
        wanderState = new GridEnemyWander(this, null, 10, pathFinding);
        chaseState = new GridEnemyChase(this, null, pathFinding);
        attackState = new GridEnemyAttack(this, null, ability);
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

    public void Dash(HexCell cellToDash)
    {
        EnemyManager.Instance.ReleaseCell(this);
        var nextGridPosition = cellToDash.ParentComponent.transform.position;
        Vector3 directionToNextGrid = (nextGridPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToNextGrid);
        transform.DOMove(cellToDash.ParentComponent.transform.position, enemyActor.ActionCooldown).SetEase(Ease.InBack);
        EnemyManager.Instance.OnMove(this, cellToDash.Coordinates);
        transform.DORotateQuaternion(targetRotation, enemyActor.ActionCooldown);
        currentCoord = cellToDash.Coordinates;
        currentCell = cellToDash;
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