using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;

public class EnemyManager : Singleton<EnemyManager>
{
	[SerializeField] private AIBrain enemyPrefab;
	[SerializeField] private List<Vector2Int> spawnCoords = new List<Vector2Int>();
	[SerializeField] private Transform enemyGroup;

	private Dictionary<AIBrain, Vector3Int> enemiesDict = new Dictionary<AIBrain, Vector3Int>();
	public Action<AIBrain,Vector3Int> OnMove;

	private void Start()
	{
		OnMove += EnemyCatcher;
	}

	public void InitEnemies()
	{
		foreach (var coord in spawnCoords)
		{
			HexCellComponent cell = BattleManager.Instance.hexgrid.GetCellInCoord(new Vector3Int(coord.x, 0, coord.y));
			if (cell.CellData.CellType == CellType.Empty)
			{
				AIBrain newInstance = Instantiate(enemyPrefab, cell.transform.position, quaternion.identity, enemyGroup);
				newInstance.currentCoord = cell.CellData.Coordinates;
				enemiesDict.Add(newInstance,newInstance.currentCoord);
				cell.CellData.SetType(CellType.Enemy);
			}
			else
			{
				Debug.LogError("Not valid cell to spawn!");
			}
		}
		
	}

	public void EnemyCatcher(AIBrain enemy,Vector3Int targetCoord)
	{
		HexCellComponent oldCell = BattleManager.Instance.hexgrid.GetCellInCoord(enemy.currentCoord);
		oldCell.CellData.SetType(CellType.Empty);
		HexCellComponent targetCell = BattleManager.Instance.hexgrid.GetCellInCoord(targetCoord);

		if(enemiesDict.ContainsKey(enemy))
		{
			enemiesDict[enemy] = targetCoord;
		}
		targetCell.CellData.SetType(CellType.Enemy);
	}
}