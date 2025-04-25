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
	[SerializeField] private Image IconOfBeReplacedAbility;
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
			var b = BattleManager.Instance;
			var c = b.hexgrid.GetCellInCoordVector2(pos);
			if (!b.hexgrid.CheckCellInRange(b.PlayerCell, c, 1))
			{
				int randValue = (int)Mathf.Floor(Random.value);
				if(randValue <= legendaryChestRate) chestMap.Add(pos, ChestType.Legend);
				else if (randValue <= goldChestRate) chestMap.Add(pos, ChestType.Gold);
				else if (randValue <= steelChestRate) chestMap.Add(pos, ChestType.Steel);
			}
			
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

	void RandomizeOption(HexDirection enterDirection)
	{
		List<AbilityData> selectedAbilities = new List<AbilityData>();
    
		foreach (var ob in optionBehaviours)
		{
			AbilityData a = null;
			int attempts = 0;
        
			while ((a == null || selectedAbilities.Contains(a)) && attempts < 10)
			{
				a = BattleManager.Instance.abilityDatabase.GetRandomAbilityFromList("main");
				attempts++;
			}
			
			if (a != null && !selectedAbilities.Contains(a) || selectedAbilities.Count >= 3)
			{
				if (!selectedAbilities.Contains(a))
				{
					selectedAbilities.Add(a);
				}
				
				ob.Set(a.Icon, a.Title, a.Desc, () =>
				{
					EquippedAbilityManager.RemoveAndReplaceAbilityInDirection(enterDirection,a);
					PlayerActionHudController.Instance.RefreshHUD();
				});
			}
		}
	}
	
	public void EnableChestUICanvas(HexCell chestCell,HexDirection enterDirection)
	{
		//pause the game time
		CurrentChest = chestCell.StandingGameObject;
		chestCell.SetCell(null, CellType.Empty);
		ChestUICanvas.gameObject.SetActive(true);
		IconOfBeReplacedAbility.sprite = EquippedAbilityManager.GetEquippedAbilityData(enterDirection).Icon;
		RandomizeOption(enterDirection);
		Time.timeScale = 0;
	}

	public void DisableChestUICanvas()
	{
		//resume the game time
		Destroy(CurrentChest);
		CurrentChest = null;
		ChestUICanvas.gameObject.SetActive(false);
		BattleManager.Instance.UpdateValidMoveRange();
		Time.timeScale = 1;
	}
}