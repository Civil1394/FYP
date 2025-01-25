using UnityEngine;
using System.Collections.Generic;

public class GridEnemyRetreat : EnemyBaseState
{
    PathFinding pathFinding;
    List<HexCell> path;
    int pathProgress = 0;
    bool isPlayerMoved = false;
    int retreatDistance = 0;
    public GridEnemyRetreat(AIBrain enemyBrain, Animator animator, PathFinding pathFinding, int retreatDistance) : base(enemyBrain, animator)
    {
        this.pathFinding = pathFinding;
        this.retreatDistance = retreatDistance;
    }
    public override void OnEnter()
    {
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
        //check the distance with player
        //decide whether continue to retreat or go back to chase
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

        Vector3 playerCell = enemyBrain.playerGrid.transform.position;
        Vector3 escapeDir = enemyBrain.transform.position - playerCell;

        HexCellComponent end =
            BattleManager.Instance.hexgrid.GetAvailableCellByWorldDirection(enemyBrain.playerGrid,
                escapeDir, retreatDistance);
        if (!end)
        {
            Debug.Log("the end point is null");
            return;
        }

        path = await pathFinding.FindPathAsync(start, end);
        enemyBrain.gPath = path;
    }
}