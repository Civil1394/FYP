using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;

public class BattleManager : Singleton<BattleManager>
{
	[Header("Player Stats")]
	[SerializeField] private int playerHealth = 10;
	
	[Header("Player-related ref")] 
	[SerializeField] private GameObject playerPrefab;
	[SerializeField] private Vector2Int playerSpawnCoord;
	[SerializeField] private CinemachineVirtualCamera playerCamera;
	public UIHourGlassView playerUIHourGlassView;
	public PlayerActor PlayerActorInstance {  get; private set; }
	public AbilityDatabase AbilityDatabase;
	public HexCellComponent PlayerCell;
	[Header("HexGrid Related Ref")]
	public HexGrid hexgrid = new HexGrid();

	[Header("Turn Related Ref")]
	[SerializeField] private float initTurnDur = 0.5f;
	public float InitTurnDur
	{
		get => initTurnDur;
	}
	[Header("HandCard Related Ref")] 
	public int handCardsSize = 2;
	
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
	public InputHandler InputHandler
	{
		get => _inputHandler;
		private set => _inputHandler = value;
	}
	
	//CastingHandler
	// [SerializeField] private CastingHandler _castingHandler;
	// public CastingHandler CastingHandler
	// {
	// 	get => _castingHandler;
	// 	private set => _castingHandler = value;
	// }
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
		
		
		//InitPlayer
		InitPlayer();
		
		//InitEnemies
		EnemyManager.Instance.InitEnemies();
		
		//InitTurn
		
		TurnManager.OnActionExecuted += HandleActionExecuted;
		IsBattleStarted = true;
		//StartCoroutine(_TurnBaseCoroutine());
		
		//init player valid move range
		HexCellComponent[] newNearbyCells = hexgrid.GetCellsInRange(hexgrid.GetCellByType(CellType.Player), 1);
		foreach (var cell in newNearbyCells)
		{
			if (cell.CellData.CellType == CellType.Empty)
			{
				cell.CellData.SetGuiType(CellGuiType.ValidMoveRange);
			}
		}
		
	}
	
	private void InitPlayer()
	{
		//Spawn player at random empty cell
		HexCellComponent cell = hexgrid.GetCellInCoord(new Vector3Int(playerSpawnCoord.x, 0, playerSpawnCoord.y));
		if (cell.CellData.CellType == CellType.Empty)
		{
			GameObject newInstance = 
				Instantiate(playerPrefab, new Vector3(cell.transform.position.x,playerPrefab.transform.position.y,cell.transform.position.z)
					, quaternion.identity);
			
			PlayerCell = cell;
			//inputHandler = newInstance.GetComponent<InputHandler>();
			PlayerActorInstance = newInstance.GetComponent<PlayerActor>();
			cell.CellData.SetCell(PlayerActorInstance.gameObject,CellType.Player);
			playerCamera.Follow = PlayerActorInstance.transform;
			playerCamera.LookAt = PlayerActorInstance.transform;
			PlayerActorInstance.ActionCooldown = initTurnDur;
		}
		else
		{
			Debug.LogError("Not valid cell to spawn!");
		}
		hexgrid.UpdatePlayerSixDirCellsSet();
	}
	#endregion
	
	//Update Cell states after player Move
	public void OnPlayerMove(HexCellComponent oldCell, HexCellComponent newCell)
	{
		// Clear old valid move ranges
		HexCellComponent[] oldNearbyCells = BattleManager.Instance.hexgrid.GetCellsInRange(oldCell, 1);
		foreach (var cell in oldNearbyCells)
		{
			cell.CellData.SetGuiType(CellGuiType.Empty);
		}

		// Update cell types
		oldCell.CellData.ClearCell();
		newCell.CellData.SetCell(PlayerActorInstance.gameObject,CellType.Player);
		PlayerCell = newCell;
		// Set new valid move ranges
		HexCellComponent[] newNearbyCells = BattleManager.Instance.hexgrid.GetCellsInRange(newCell, 1);
		foreach (var cell in newNearbyCells)
		{
			if (cell.CellData.CellType == CellType.Empty)
			{
				cell.CellData.SetGuiType(CellGuiType.ValidMoveRange);
			}
		}
		//Update Valid cells set of six direction from player
		hexgrid.UpdatePlayerSixDirCellsSet();
		
		TurnManager.ExecuteAction(TurnActionType.Move,
			$"Moved from {oldCell.CellData.Coordinates} to {newCell.CellData.Coordinates}");

	}
	private void HandleActionExecuted(TurnAction action)
	{
		Debug.Log($"Action executed: {action.ActionType} - {action.Description}");
		actionExecuted = true;
	}
}



