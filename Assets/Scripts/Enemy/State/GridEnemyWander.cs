using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridEnemyWander : EnemyBaseState
{
    readonly Vector3 startPoint;
    readonly float wanderRadius;
    List<HexCell> path;
    int pathProgress = 0;
    public GridEnemyWander(AIBrain enemyBrain, Animator animator, float wanderRadius) : base(enemyBrain, animator)
    {
        startPoint = enemyBrain.transform.position;
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
        if(!HasReachedDestination())
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
        HexCellComponent start = BattleManager.Instance.hexgrid.GetCell(enemyBrain.currentCoord);
        Vector3Int randomPos = new Vector3Int((int)Random.Range(1, wanderRadius), (int)Random.Range(1, wanderRadius));
        HexCellComponent end = BattleManager.Instance.hexgrid.GetCell(enemyBrain.currentCoord);
        PathFinding pathFinding = new PathFinding(start, end);
        path = await pathFinding.FindPathAsync();
        // Use the path (e.g., move a character along it)
    }
}