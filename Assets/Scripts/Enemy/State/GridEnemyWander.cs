using System.Collections.Generic;
using UnityEngine;

public class GridEnemyWander : EnemyBaseState
{
    PathFinding pathFinding;
    readonly float wanderRadius;
    List<HexCell> path;
    int pathProgress = 0;

    public GridEnemyWander(AIBrain enemyBrain, Animator animator, float wanderRadius, PathFinding pathFinding) : base(enemyBrain, animator)
    {
        this.pathFinding = pathFinding;
        this.wanderRadius = wanderRadius;
    }
    public override void OnEnter()
    {
        Debug.Log("start wandering");
        RunPathfindingAsync();
    }
    public override void OnExit()
    {

    }
    public override void Update()
    {

    }
    public override void FixedUpdate()
    {

    }
    public override void TurnAction()
    {
        if (path == null) return;
        if (path.Count <= 0) return;
        if (HasReachedDestination()) { RunPathfindingAsync(); return; }
        //prevent the enemy step into other enemy or player
        if (path[pathProgress + 1].CellType != CellType.Empty) { RunPathfindingAsync(); return; }
        //prevent two enemy step into the same cell at the same time
        if (!EnemyManager.Instance.ReserveCell(enemyBrain, path[pathProgress + 1])) { RunPathfindingAsync(); return; }
        enemyBrain.Move(path[++pathProgress]);
    }
    bool HasReachedDestination()
    {
        return pathProgress >= path.Count - 1;
    }
    private async void RunPathfindingAsync()
    {
        pathProgress = 0;
        if (path == null) { path = new List<HexCell>(); }
        else { path.Clear(); }
        HexCellComponent start = BattleManager.Instance.hexgrid.GetCellInCoord(enemyBrain.currentCoord);
        HexCellComponent end = GetRandomTargetPos();
        path = await pathFinding.FindPathAsync(start, end);
        enemyBrain.gPath = path;
    }
    private HexCellComponent GetRandomTargetPos()
    {
        Vector3Int randomPos;
        do
        {
            randomPos = new Vector3Int((int)UnityEngine.Random.Range(-wanderRadius, wanderRadius), 0, (int)UnityEngine.Random.Range(-wanderRadius, wanderRadius));
            randomPos += enemyBrain.currentCoord;
        } while (!BattleManager.Instance.hexgrid.HasCell(randomPos));
        //Debug.Log(randomPos.ToString());
        return BattleManager.Instance.hexgrid.GetCellInCoord(randomPos);
    }
}