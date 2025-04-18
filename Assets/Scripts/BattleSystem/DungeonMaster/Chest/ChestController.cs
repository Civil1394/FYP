using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ChestController : MonoBehaviour
{
	public enum ChestType
	{
		Steel = 0,
		Gold = 1,
		Legend = 2
	}
	[SerializeField] private Canvas ChestUICanvas;
	[SerializeField] private List<OptionBehaviour> optionBehaviours;
	public GameObject CurrentChest;
	[SerializeField] private GameObject steelChestPrefab;
	[Range(0,1)] public float steelChestRate;
	[Range(0,1)] public float goldChestRate;
	[Range(0,1)] public float legendaryChestRate;
	public int maxChestCount;
	public Vector2Int[] staticLegendaryChestLocation;

	private void Start()
	{
		
	}

	public void InitializeChests()
	{
		SpawnChest();
		foreach (var ob in optionBehaviours)
		{
			ob.btn.onClick.AddListener(DisableChestUICanvas);
		}
	}
	public Dictionary<Vector2Int, ChestType> GetChestHeatMap(int width, int height)
	{
		Dictionary<Vector2Int, ChestType> chestMap = new Dictionary<Vector2Int, ChestType>();
		List<Vector2Int> rawChestLocations = NoiseSystem.GetPositions(maxChestCount, width, height);
		foreach (Vector2Int pos in rawChestLocations)
		{
			int randValue = (int)Mathf.Floor(Random.value);
			if(randValue <= legendaryChestRate) chestMap.Add(pos, ChestType.Legend);
			else if (randValue <= goldChestRate) chestMap.Add(pos, ChestType.Gold);
			else if (randValue <= steelChestRate) chestMap.Add(pos, ChestType.Steel);
		}
		return chestMap;
	}

	public void SpawnChest()
	{
		int h = BattleManager.Instance.hexgrid.Height;
		int w = BattleManager.Instance.hexgrid.Width;
		Dictionary<Vector2Int, ChestType> chestTypeMap = GetChestHeatMap(w, h);
		foreach (var p in chestTypeMap)
		{
			//should spawn a chest on top of the cell and set the chest type to chest
			var cell = BattleManager.Instance.hexgrid.GetCellInCoordVector2(p.Key);
			GameObject tempChest = Instantiate(steelChestPrefab,cell.transform.position,Quaternion.identity);
			tempChest.transform.SetParent(transform);
			cell.CellData.SetGuiType(CellActionType.Chest);
			cell.CellData.SetCell(tempChest, CellType.Invalid);
		}
	}

	void RandomizeOption()
	{
		foreach (var ob in optionBehaviours)
		{
			//ob.Set();
		}
	}
	
	public void EnableChestUICanvas(HexCell chestCell)
	{
		//pause the game time
		CurrentChest = chestCell.StandingGameObject;
		chestCell.SetCell(null, CellType.Empty);
		ChestUICanvas.gameObject.SetActive(true);
		RandomizeOption();
		Time.timeScale = 0;
	}

	public void DisableChestUICanvas()
	{
		//resume the game time
		Destroy(CurrentChest);
		CurrentChest = null;
		ChestUICanvas.gameObject.SetActive(false);
		Time.timeScale = 1;
	}
}