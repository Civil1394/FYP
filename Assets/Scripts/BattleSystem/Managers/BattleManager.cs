using System;
using UnityEngine;
using System.Collections;
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
	public HourglassUIAnimator playerHourglassUIAnimator;
	public List<PlayerActor> PlayerActorInstance {  get; private set; } = new List<PlayerActor>();
	public AbilityDatabase AbilityDatabase;
	public HexCellComponent PlayerCell;
	[Header("HexGrid Related Ref")]
	public HexGrid hexgrid = new HexGrid();
	
	

	[Header("Hourglass Related Ref")]
	[SerializeField] private float initSandAmount = 0.5f;
	public float InitSandAmount
	{
		get => initSandAmount;
	}
	
	[Header("HandCard Related Ref")] 
	public int handCardsSize = 2;

	[Header("Enemy Related Ref")] 
	public EnemyDatabase EnemyDatabase;
	public bool IsBattleStarted = false;
	
	#region Manager References
	[Header("Managers Related Ref")]
	//TurnManager
	[SerializeField] TurnManager _turnManager;
	public TurnManager TurnManager
	{
		get => _turnManager;
		private set => _turnManager = value;
		
	}
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

	//ChainManager
	public ChainManager chainManager { get; private set; }
	[SerializeField] private bool actionExecuted = false;
	protected override void Awake()
	{
		base.Awake();
		if(_turnManager == null) _turnManager = new TurnManager();
	}
	#endregion
	private void Start()
	{
		InitBattle();
	}
	#region Init

	public void InitBattle()
	{
		if (AbilityDatabase == null)
		{
			Debug.LogError("AbilityDatabase is not assigned to BattleManager!");
			return;
		}
		//Add cards to hand at start of combat
		for (int i = 0; i < handCardsSize; i++)
		{
			Card testCard = CardFactory.Instance.CreateCardFromList(AbilityDatabase, "1",
				AbilityDatabase.GetRandomAbilityFromList("1").id);
			CardsManager.Instance.AddCardToDeck(testCard);
			CardsManager.Instance.DrawCard();
		}
		
		//1.InitHourglasses
		InitHourglasses();
		
		//2.InitPlayer
		foreach (Vector2Int p in playerSpawnCoord)
		{
			InitPlayer(p);
		}
		
		//3.InitEnemies
		EnemyManager.Instance.InitEnemies();
		
		IsBattleStarted = true;
		
		//init player valid move range
		UpdateValidMoveRange();

		
	}

	private void InitHourglasses()
	{
		HourglassInventory.Instance.hourglassesList = HourglassFactory.Instance.CreateHourglasses(5, true,null,null,null);
		
		//int slotsAmount = HourglassesUIContainer.Instance.SlotsAmount;
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
			playerActor.Init(hg,PlayerCell);
			
			HourglassesUIContainer.Instance.InitHourglassProducts(hg);
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
		
		TurnManager.ExecuteAction(TurnActionType.Move,
			$"Moved from {previousStandingCell.CellData.Coordinates} to {newStandingCell.CellData.Coordinates}");

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
	
	private void HandleActionExecuted(TurnAction action)
	{
		Debug.Log($"Action executed: {action.ActionType} - {action.Description}");
		actionExecuted = true;
	}
}



