using System.Collections.Generic;
using UnityEngine;

public class GridEnemyWander : EnemyBaseState
{
    readonly float wanderRadius;
    List<HexCell> path;
    List<Vector3> pathLine;
    int pathProgress = 1;

    public GridEnemyWander(AIBrain enemyBrain, Animator animator, float wanderRadius) : base(enemyBrain, animator)
    {
        this.wanderRadius = wanderRadius;
    }
    public override void OnEnter()
    {
        pathLine = new List<Vector3>();
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
        if (HasReachedDestination())
        {
            //Debug.Log("arrived");
            RunPathfindingAsync();
            pathProgress = 1;
        }
        else
        {
            enemyBrain.Move(path[pathProgress++]);
        }
    }
    bool HasReachedDestination()
    {
        return pathProgress>=path.Count;
    }
    private async void RunPathfindingAsync()
    {
        path?.Clear();
        HexCellComponent start = BattleManager.Instance.hexgrid.GetCellInCoord(enemyBrain.currentCoord);
        HexCellComponent end = GetRandomTargetPos();
        PathFinding pathFinding = new PathFinding(start, end);
        path = await pathFinding.FindPathAsync();
        // Use the path (e.g., move a character along it)
    }
    private HexCellComponent GetRandomTargetPos()
    {
        Vector3Int randomPos;
        do
        {
            randomPos = new Vector3Int((int)UnityEngine.Random.Range(-wanderRadius, wanderRadius), 0, (int)UnityEngine.Random.Range(-wanderRadius, wanderRadius));
            randomPos += enemyBrain.currentCoord;
        }while (!BattleManager.Instance.hexgrid.HasCell(randomPos));
        //Debug.Log(randomPos.ToString());
        return BattleManager.Instance.hexgrid.GetCellInCoord(randomPos);
    }
    void GetCellTransform()
    {
        pathLine.Clear();
        if (path == null) return;
        foreach (HexCell cell in path)
        {
            pathLine.Add(BattleManager.Instance.hexgrid.GetCellInCoord(cell.Coordinates).transform.position);
        }
    }
}