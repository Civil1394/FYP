using System.Collections.Generic;
using UnityEngine;

public class GridEnemyChase : EnemyBaseState
{
    PathFinding pathFinding;
    List<HexCell> path;
    int pathProgress = 0;
    bool isPlayerMoved = false;

    public GridEnemyChase(AIBrain enemyBrain, Animator animator, PathFinding pathFinding) : base(enemyBrain, animator)
    {
        this.pathFinding = pathFinding;
    }

    public override void OnEnter()
    {
        RunPathfindingAsync();
        Debug.Log("start chase");
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
        if (HasReachedDestination()){ RunPathfindingAsync(); return; }
        //prevent the enemy step into other enemy or player
        if (path[pathProgress + 1].CellType != CellType.Empty) { RunPathfindingAsync(); return; }
        //prevent two enemy step into the same cell at the same time
        if (!EnemyManager.Instance.ReserveCell(enemyBrain, path[pathProgress + 1])) { RunPathfindingAsync(); return; }
        enemyBrain.Move(path[++pathProgress]);
    }
    public bool HasReachedDestination()
    {
        return pathProgress >= path.Count - 1;
    }
    private async void RunPathfindingAsync()
    {
        pathProgress = 0;
        if (path == null) { path = new List<HexCell>(); }
        else { path.Clear(); }
        HexCellComponent start = enemyBrain.currentCell.ParentComponent;
            //BattleManager.Instance.hexgrid.GetCellInCoord(enemyBrain.currentCoord);
        HexCellComponent end = enemyBrain.playerGrid ? enemyBrain.playerGrid : enemyBrain.lastSeenPlayerGrid;
        if (!end) return;
        path = await pathFinding.FindPathAsync(start, end);
        enemyBrain.gPath = path;
    }
}
