using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ChestSpawner : MonoBehaviour
{
	public enum ChestType
	{
		Wood = 0,
		Steel = 1,
		Gold = 2,
		Legend = 3
	}
	
	[Range(0,1)] public float woodChestRate;
	[Range(0,1)] public float steelChestRate;
	[Range(0,1)] public float goldChestRate;
	[Range(0,1)] public float legendaryChestRate;
	public int maxChestCount;
	public Vector2Int[] staticLegendaryChestLocation;

	private void Start()
	{
		SpawnChest();
	}

	public Dictionary<Vector2Int, ChestType> GetChestHeatMap(int width, int height)
	{
		Dictionary<Vector2Int, ChestType> chestMap = new Dictionary<Vector2Int, ChestType>();
		float[,] rawChestMap = NoiseSystem.GenerateNoiseMap(width, height, 0.1f);
		List<Vector2Int> rawChestLocations = NoiseSystem.GetCenterPosition(rawChestMap, 10, 5);
		foreach (Vector2Int pos in rawChestLocations)
		{
			int randValue = (int)Mathf.Floor(Random.value * 100);
			if(randValue <= legendaryChestRate) chestMap.Add(pos, ChestType.Legend);
			else if(randValue<=goldChestRate) chestMap.Add(pos, ChestType.Gold);
			else if(randValue<=steelChestRate) chestMap.Add(pos, ChestType.Steel);
			else if(randValue<=woodChestRate) chestMap.Add(pos, ChestType.Wood);
		}
		return chestMap;
	}

	public void SpawnChest()
	{
		int h = BattleManager.Instance.hexgrid.Height;
		int w = BattleManager.Instance.hexgrid.Width;
		print(h+" "+" "+w);
		foreach (var p in GetChestHeatMap(w, h))
		{
			var cell = BattleManager.Instance.hexgrid.GetCellInCoordVector2(p.Key);
			cell.CellData.SetGuiType(CellGuiType.ValidAttackCell);
		}
	}
}