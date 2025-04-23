using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;


public class EnemyManager : Singleton<EnemyManager>
{
	[SerializeField] private AIBrain enemyPrefab;
	[SerializeField] private AIBrain bossPrefab;
	[SerializeField] private EnemyData bossConfig;
	[SerializeField] private List<Vector2Int> spawnCoords = new List<Vector2Int>();
	[SerializeField] private Transform enemyGroup;

	private Dictionary<AIBrain, Vector3Int> enemiesDict = new Dictionary<AIBrain, Vector3Int>();
    private Dictionary<AIBrain, HexCell> enemyReservations = new Dictionary<AIBrain, HexCell>();

    public Action<AIBrain,Vector3Int> OnMove;


    [SerializeField] private bool IsSpawnEnemy;

    [SerializeField] private EnemyWaveController enemyWaveController;

    [Header("Wave setting")] [SerializeField]
    private float waveDuration;
	private void Start()
	{
		OnMove += EnemyCatcher;
	}

	public void InstantiateEnemy(Vector2Int coord)
	{
		HexCellComponent cell = BattleManager.Instance.hexgrid.GetCellInCoord(new Vector3Int(coord.x, 0, coord.y));
		if (cell.CellData.CellType == CellType.Empty)
		{
			AIBrain newInstance = Instantiate(enemyPrefab, cell.transform.position, quaternion.identity, enemyGroup);
			ReserveCell(newInstance, cell.CellData);
			
			var hg = HourglassInventory.Instance.GetRandomUnoccupiedHourglassFromInventory();
			newInstance.gameObject.GetComponent<EnemyActor>().Init(hg);
			if (BattleManager.Instance.EnemyDatabase == null)
			{
				print("database is null");
			}

			var ed = BattleManager.Instance.EnemyDatabase.GetRandomEnemyFromList("main");
			newInstance.Init(ed,cell.CellData);
			newInstance.GetComponent<IDamagable>().InitIDamagable(ed.Health);
			enemiesDict.Add(newInstance,newInstance.currentCoord);
			cell.CellData.SetCell(newInstance.gameObject,CellType.Enemy);
		}
		else
		{
			Debug.LogError("Not valid cell to spawn!");
		}
	}
	public void InitEnemies()
	{
		StartCoroutine(enemyWaveController.EnemyWave(5, waveDuration));
		StartCoroutine(EnemyMonitor());
		// if (IsSpawnEnemy)
		// {
		// 	foreach (var coord in spawnCoords)
		// 	{
		// 		HexCellComponent cell = BattleManager.Instance.hexgrid.GetCellInCoord(new Vector3Int(coord.x, 0, coord.y));
		// 		if (cell.CellData.CellType == CellType.Empty)
		// 		{
		// 			AIBrain newInstance = Instantiate(enemyPrefab, cell.transform.position, quaternion.identity, enemyGroup);
		// 			var hg = HourglassInventory.Instance.GetRandomUnoccupiedHourglassFromInventory();
		// 			newInstance.gameObject.GetComponent<EnemyActor>().Init(hg);
		// 			
		// 			newInstance.currentCoord = cell.CellData.Coordinates;
		// 			newInstance.currentCell = cell.CellData;
		// 			enemiesDict.Add(newInstance,newInstance.currentCoord);
		// 			cell.CellData.SetCell(newInstance.gameObject,CellType.Enemy);
		// 		}
		// 		else
		// 		{
		// 			print(coord.ToString());
		// 			Debug.LogError("Not valid cell to spawn!");
		// 			continue;
		// 		}
		// 	}
		// }

		//StartCoroutine(enemyWaveController.EnemyWave(10, 10f));
	}

	public IEnumerator EnemyMonitor()
	{
		float timeSinceLastWave = 0f;
    
		while (enemyWaveController.waveCnt < 4)
		{
			timeSinceLastWave += 10f; 
        
			// Start a new wave if no enemies remain OR if we've waited too long
			if (enemiesDict.Count < 1 || timeSinceLastWave >= waveDuration)
			{
				timeSinceLastWave = 0f;
            
				if (enemyWaveController.waveCnt == 3)
				{
					StartCoroutine(enemyWaveController.BossEnemyWave(10, 5));
				}
				StartCoroutine(enemyWaveController.EnemyWave(20, 40));
			}
        
			yield return new WaitForSeconds(10);
		}
	}

	public void InstantiateBoss()
	{
		Vector2Int coord = Vector2Int.zero;
		HexCellComponent cell = BattleManager.Instance.hexgrid.GetCellInCoord(new Vector3Int(coord.x, 0, coord.y));
		if (cell.CellData.CellType == CellType.Empty)
		{
			AIBrain newInstance = Instantiate(bossPrefab, cell.transform.position, quaternion.identity, enemyGroup);
			ReserveCell(newInstance, cell.CellData);
			
			var hg = HourglassInventory.Instance.GetRandomUnoccupiedHourglassFromInventory();
			newInstance.gameObject.GetComponent<EnemyActor>().Init(hg);
			
			newInstance.Init(bossConfig,cell.CellData);
			enemiesDict.Add(newInstance,newInstance.currentCoord);
			cell.CellData.SetCell(newInstance.gameObject,CellType.Enemy);
		}
		else
		{
			Debug.LogError("Not valid cell to spawn!");
		}
	}
    public bool ReserveCell(AIBrain enemy, HexCell cell)
    {
		if (IsCellReserved(cell)) return false;
        enemyReservations[enemy] = cell;
		return true;
    }

    public void ReleaseCell(AIBrain enemy)
    {
	    if (enemiesDict.ContainsKey(enemy))
	    {
		    HexCellComponent targetCell = BattleManager.Instance.hexgrid.GetCellInCoord(enemiesDict[enemy]);
		    targetCell.CellData.SetCell(null, CellType.Empty);
	    }
    }
    public void ReleaseReservationCell(AIBrain enemy)
    {
	    if (enemyReservations.ContainsKey(enemy))
	    {
		    enemyReservations.Remove(enemy);
	    }
    }

    public bool IsCellReserved(HexCell cell)
    {
        return enemyReservations.ContainsValue(cell);
    }

    public void UnregisterFromDict(AIBrain enemy)
    {
	    if(enemiesDict.ContainsKey(enemy)) enemiesDict.Remove(enemy);
    }
    public void EnemyCatcher(AIBrain enemy,Vector3Int targetCoord)
	{
		HexCellComponent oldCell = BattleManager.Instance.hexgrid.GetCellInCoord(enemy.currentCoord);
		oldCell.CellData.ClearCell();
		HexCellComponent targetCell = BattleManager.Instance.hexgrid.GetCellInCoord(targetCoord);

		if(enemiesDict.ContainsKey(enemy))
		{
			enemiesDict[enemy] = targetCoord;
		}

		targetCell.CellData.SetCell(enemy.gameObject, CellType.Enemy);
	}
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
		foreach (var c in enemyReservations)
		{
			var temp = BattleManager.Instance.hexgrid.GetCellInCoord(c.Value.Coordinates);
			Gizmos.DrawCube(temp.transform.position, Vector3.one);
		}
    }

    protected override void OnDestroy()
    {
	    base.OnDestroy();
	    StopAllCoroutines();
    }
}