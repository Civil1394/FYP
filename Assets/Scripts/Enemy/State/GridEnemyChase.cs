using System.Collections.Generic;
using UnityEngine;

public class GridEnemyChase : EnemyBaseState
{
    List<HexCell> path;
    List<Vector3> pathLine;
    int pathProgress = 0;
    bool isPlayerMoved = false;

    public GridEnemyChase(AIBrain enemyBrain, Animator animator) : base(enemyBrain, animator)
    {
    }

    public override void OnEnter()
    {
        pathLine = new List<Vector3>();
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
        if (HasReachedDestination())
        {
            RunPathfindingAsync();
            return;
        }
        if (path[pathProgress + 1].CellType != CellType.Empty) { RunPathfindingAsync(); return; }
        if (!EnemyManager.Instance.ReserveCell(enemyBrain, path[pathProgress + 1])) { RunPathfindingAsync(); return; }
        else
        {
            enemyBrain.Move(path[++pathProgress]);
        }
    }
    public bool HasReachedDestination()
    {
        return pathProgress >= path.Count - 1;
    }
    private async void RunPathfindingAsync()
    {
        pathProgress = 0;
        if (path == null)
        {
            path = new List<HexCell>();
        }
        else
        {
            path.Clear();
        }
        HexCellComponent start = BattleManager.Instance.hexgrid.GetCellInCoord(enemyBrain.currentCoord);
        HexCellComponent end = enemyBrain.playerGrid ? enemyBrain.playerGrid : enemyBrain.lastSeenPlayerGrid;
        if (!end) return;

        PathFinding pathFinding = new PathFinding(start, end);
        path = await pathFinding.FindPathAsync();
        enemyBrain.gPath = path;

        //if (!EnemyManager.Instance.ReserveCell(enemyBrain, path[pathProgress]))
        //{
        //    path = await pathFinding.FindPathAsync();
        //}
    }
}
