using System.Collections.Generic;
using UnityEngine;

public class GridEnemyChase : EnemyBaseState
{
    List<HexCell> path;
    List<Vector3> pathLine;
    int pathProgress = 1;
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
        if (pathProgress < path.Count)
        {
            enemyBrain.Move(path[pathProgress++]);
        }
        else
        {
            RunPathfindingAsync();
        }
    }

    private async void RunPathfindingAsync()
    {
        pathProgress = 1;
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
        GetCellTransform();
    }

    void GetCellTransform()
    {
        pathLine.Clear();
        if (path == null) return;
        foreach (HexCell cell in path)
        {
            pathLine.Add(BattleManager.Instance.hexgrid.GetCellInCoord(cell.Coordinates).transform.position);
        }
        enemyBrain.pathLine = pathLine;
    }
}
