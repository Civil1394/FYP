using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridEnemyChase : EnemyBaseState
{
    private List<HexCellComponent> path;
    public GridEnemyChase(AIBrain enemyBrain, Animator animator) : base(enemyBrain, animator)
    {
    }
    public override void OnEnter()
    {
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

    }
    bool HasReachedDestination()
    {
        return false;

    }
    private async void RunPathfindingAsync()
    {
        HexCellComponent start = BattleManager.Instance.hexgrid.GetCell(enemyBrain.currentCoord);
        HexCellComponent end = BattleManager.Instance.hexgrid.GetCell(enemyBrain.currentCoord);
        PathFinding pathFinding = new PathFinding(start, end);
        List<HexCell> path = await pathFinding.FindPathAsync();

        // Use the path (e.g., move a character along it)
    }
}