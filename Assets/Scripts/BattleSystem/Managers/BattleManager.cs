using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;

public class BattleManager : Singleton<BattleManager>
{
	[Header("Player Stats")]
	[SerializeField] private int validMoveRange = 2;
	
	[Header("Player-related ref")] 
	[SerializeField] private GameObject playerPrefab;
	[SerializeField] private List<Vector2Int> playerSpawnCoord = new List<Vector2Int>();
	[SerializeField] private CinemachineVirtualCamera playerCamera;
	[SerializeField] private HourglassOnHudAnimator hourglassOnHudAnimator;
	public List<PlayerActor> PlayerActorInstance {  get; private set; } = new List<PlayerActor>();
	public HexCellComponent PlayerCell;
	
	[Header("HexGrid Related Ref")]
	public HexGrid hexgrid = new HexGrid();
	
	[Header("HandCard Related Ref")] 
	public int handCardsSize = 2;

	[Header("Enemy Related Ref")] 
	public EnemyDatabase EnemyDatabase;
	public bool IsBattleStarted = false;
	
	[SerializeField] private AbilityDatabase abilityDatabase;
	#region Manager References
	[Header("Managers Related Ref")]

	//InputHandler
	[SerializeField] private InputHandler _inputHandler;
	[SerializeField] private MultipleCharacterControlSystem _multipleCharacterControlSystem;
	public InputHandler InputHandler
	{
		get => _inputHandler;
		private set => _inputHandler = value;
	}
	public MultipleCharacterControlSystem MultipleCharacterControlSystem
	{
		get => _multipleCharacterControlSystem;
		private set => _multipleCharacterControlSystem = value;
	}
	
	[SerializeField] private bool actionExecuted = false;
	protected override void Awake()
	{
		base.Awake();
	}
	#endregion
	private void Start()
	{
		InitBattle();
	}
	#region Init

	public void InitBattle()
	{
		//init Player base abilities
		EquippedAbilityManager.InitAbiltiyDatabase(abilityDatabase);
		
		//InitHourglasses
		InitHourglasses();
		
		//InitPlayer
		foreach (Vector2Int p in playerSpawnCoord)
		{
			InitPlayer(p);
		}
		
		//InitEnemies
		EnemyManager.Instance.InitEnemies();
		
		IsBattleStarted = true;
		
		//init player valid move range
		UpdateValidMoveRange();
		
		
		
		
	}

	private void InitHourglasses()
	{
		HourglassInventory.Instance.hourglassesList = HourglassFactory.Instance.CreateHourglasses(5, true,null,null,null);
		
		//int slotsAmount = HourglassesUIContainer.Instance.hourglassSlotsCount;
		// Hourglass[] hourglassesArray = new Hourglass[slotsAmount];
		// for (int i = 0; i < slotsAmount; i++)
		// {
		// 	hourglassesArray[i] =  HourglassInventory.Instance.GetRandomUnoccupiedHourglassFromInventory();
		// 	
		// }
		//
		// HourglassesUIContainer.Instance.InitHourglassProducts(hourglassesArray);
	}
	
	private void InitPlayer(Vector2Int playerPos)
	{
		//Spawn player at random empty cell
		HexCellComponent cell = hexgrid.GetCellInCoord(new Vector3Int(playerPos.x, 0, playerPos.y));
		if (cell.CellData.CellType == CellType.Empty)
		{
			GameObject newInstance = 
				Instantiate(playerPrefab, new Vector3(cell.transform.position.x,playerPrefab.transform.position.y,cell.transform.position.z)
					, quaternion.identity);
			
			PlayerCell = cell;
			
			PlayerActor playerActor = newInstance.GetComponent<PlayerActor>();
			var hg = HourglassInventory.Instance.GetRandomUnoccupiedHourglassFromInventory();
			playerActor.Init(hg,PlayerCell,hourglassOnHudAnimator);
			
			cell.CellData.SetCell(playerActor.gameObject,CellType.Player);
			playerCamera.Follow = playerActor.transform;
			playerCamera.LookAt = playerActor.transform;
			
			PlayerActorInstance.Add(playerActor);
			MultipleCharacterControlSystem.charactersActorList.Add(playerActor);
		}
		else
		{
			Debug.LogError("Not valid cell to spawn!");
		}
		hexgrid.UpdatePlayerSixDirCellsSet();
	}
	#endregion
	
	//Update Cell states after player Move
	public void OnPlayerMove(PlayerActor playerActor, HexCellComponent previousStandingCell, HexCellComponent newStandingCell)
	{
		// Clear old valid move ranges
		ClearValidMoveCells(previousStandingCell);

		// Update cell types
		previousStandingCell.CellData.ClearCell();
		newStandingCell.CellData.SetCell(playerActor.gameObject,CellType.Player);
		PlayerCell = newStandingCell;
		
		// Set new valid move ranges
		UpdateValidMoveRange();
		
		//Update Valid cells set of six direction from player
		hexgrid.UpdatePlayerSixDirCellsSet();
		
	}

	private void ClearValidMoveCells(HexCellComponent oldStandingCell)
	{
		HexCellComponent[] oldNearbyCells = BattleManager.Instance.hexgrid.GetCellsInRange(oldStandingCell, validMoveRange);
		foreach (var cell in oldNearbyCells)
		{
			cell.CellData.SetGuiType(CellGuiType.Empty);
		}
	}
	
	private void UpdateValidMoveRange()
	{
		HexCellComponent[] newNearbyCells = hexgrid.GetCellsInRange(hexgrid.GetCellByType(CellType.Player), validMoveRange);
		foreach (var cell in newNearbyCells)
		{
			if (cell.CellData.CellType == CellType.Empty)
			{
				cell.CellData.SetGuiType(CellGuiType.ValidMoveCell);
			}
		}
	}
	

}



