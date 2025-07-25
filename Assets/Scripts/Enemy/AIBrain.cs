using System;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIBrain : MonoBehaviour
{
    public EnemyData enemyConfig;

    [SerializeField] private MeshRenderer meshRenderer;
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

    public void Init(EnemyData ed, HexCell cell)
    {
        pathFinding = new PathFinding();
        this.enemyConfig = ed;
        currentCell = cell;
        currentCoord = cell.Coordinates;
        mColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        playerDetector.Init(this, enemyConfig.AngleOfRange, enemyConfig.DistanceOfRange, enemyConfig.InnerSphereRadius);
        StateInitialization();
    }

    public virtual void StateInitialization()
    {
        enemyAbility = enemyConfig.AbilityData.Create();
        meshRenderer.material.color = AbilityColorHelper.GetAbilityColor(enemyAbility.ColorType);
        stateMachine = new StateMachine();
        switch (enemyConfig.EnemyStateType)
        {
            case EnemyData.EnemyType.Berserk:
                BerserkState(enemyAbility);
                break;
            case EnemyData.EnemyType.Sniper:
                SniperState(enemyAbility);
                break;
            case EnemyData.EnemyType.Assassin:
                AssassinState(enemyAbility);
                break;
            case EnemyData.EnemyType.Boomer:
                BoomerState(enemyAbility);
                break;
            default:
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
        retreatState = new GridEnemyRetreat(this, null, pathFinding, enemyConfig.RetreatDistance);
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
                    BattleManager.Instance.hexgrid.GetGridDistance(currentCell.ParentComponent, playerGrid) <=
                    enemyConfig.RetreatActivateDistance
            )
        );
        stateMachine.AddTransition(
            retreatState, chaseState, new FuncPredicate(
                () =>
                    (bool)playerGrid &&
                    retreatState.HasReachedDestination()
                    //BattleManager.Instance.hexgrid.GetGridDistance(currentCell.ParentComponent, playerGrid) >= 5
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
        retreatState = new GridEnemyRetreat(this, null, pathFinding, enemyConfig.RetreatDistance);
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
                    BattleManager.Instance.hexgrid.GetGridDistance(currentCell.ParentComponent, playerGrid) <=
                    enemyConfig.RetreatActivateDistance
            )
        );
        stateMachine.AddTransition(
            retreatState, chaseState, new FuncPredicate(
                () =>
                    (bool)playerGrid &&
                    retreatState.HasReachedDestination()
                    //BattleManager.Instance.hexgrid.GetGridDistance(currentCell.ParentComponent, playerGrid) >= 5
            )
        );
        #endregion
    }
    private void Update()
    {
        stateMachine?.Update();
    }
    public void TurnAction()
    {
        stateMachine?.OnTurnStart();
        attackDur--;
        //print(attackDur);
    }

    public void ResetAttackCD()
    {
        attackDur = enemyConfig.CoolDown;
    }
    public void Move(HexCell cellToMove)
    {
        EnemyManager.Instance.ReleaseReservationCell(this);
        var nextGridPosition = cellToMove.ParentComponent.transform.position;
        Vector3 directionToNextGrid = (nextGridPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToNextGrid);
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(0,
            transform.DOJump(cellToMove.ParentComponent.transform.position, 1f, 1, enemyActor.ActionCooldown).SetEase(Ease.InOutQuad));
        sequence.Insert(0, transform.DORotateQuaternion(targetRotation, enemyActor.ActionCooldown));
        EnemyManager.Instance.OnMove(this, cellToMove.Coordinates);
        currentCoord = cellToMove.Coordinates;
        currentCell = cellToMove;
    }

    public void Dash(HexCell cellToDash, Action onFinish)
    {
        EnemyManager.Instance.ReleaseReservationCell(this);
        var nextGridPosition = cellToDash.ParentComponent.transform.position;
        Vector3 directionToNextGrid = (nextGridPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToNextGrid);
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(0, transform.DOMove(cellToDash.ParentComponent.transform.position, enemyActor.ActionCooldown).SetEase(Ease.InBack));
        sequence.Insert(0, transform.DORotateQuaternion(targetRotation, enemyActor.ActionCooldown));
        sequence.OnComplete(onFinish.Invoke);
        EnemyManager.Instance.OnMove(this, cellToDash.Coordinates);
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

    private void OnDestroy()
    {
        EnemyManager.Instance.ReleaseCell(this);
        EnemyManager.Instance.ReleaseReservationCell(this);
        EnemyManager.Instance.UnregisterFromDict(this);
        transform.DOKill();
    }
}