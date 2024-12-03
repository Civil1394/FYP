using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;

public class BattleManager : Singleton<BattleManager>
{
	[Header("Player-related ref")] 
	[SerializeField] private int playerHealth = 10;
	public Player PlayerInstance {  get; private set; }
	public AbilityDatabase AbilityDatabase;
	[SerializeField] private GameObject playerPrefab;
	[SerializeField] private Vector2Int playerSpawnCoord;
	[SerializeField] private CinemachineVirtualCamera playerCamera;
	public HexGrid hexgrid = new HexGrid();

	[Header("Turn Related Ref")]
	[SerializeField] private float initTurnDur = 0.5f;
	public float InitTurnDur
	{
		get => initTurnDur;
	}
	[Header("HandCard Related Ref")] 
	[SerializeField] private int handCardsSize = 2;
	
	public bool IsBattleStarted = false;
	public GenericAction OnTurnStart = new GenericAction();
	public Action<HexCellComponent> OnPlayerMove;
	
	[Header("Scripts Related Ref")]
	[SerializeField] TurnManager _turnManager;
	public TurnManager TurnManager
	{
		get => _turnManager;
		private set => _turnManager = value;
		
	}

	[SerializeField] private InputHandler _inputHandler;
	public InputHandler InputHandler
	{
		get => _inputHandler;
		private set => _inputHandler = value;
	}
	[SerializeField] private CastingHandler _castingHandler;
	public CastingHandler CastingHandler
	{
		get => _castingHandler;
		private set => _castingHandler = value;
	}
	public ChainManager chainManager { get; private set; }
	[SerializeField] private bool actionExecuted = false;
	protected override void Awake()
	{
		base.Awake();
		if(_turnManager == null) _turnManager = new TurnManager();
	}

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
		//InitInteraction
		//inputHandler = GetComponent<InputHandler>();
		_inputHandler.OnMoveClick.AddListener<HexCellComponent>(onPlayerMove);
		
		//InitEnemies
		EnemyManager.Instance.InitEnemies();
		
		//InitTurn
		
		TurnManager.OnActionExecuted += HandleActionExecuted;
		IsBattleStarted = true;
		StartCoroutine(_TurnBaseCoroutine());
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
			cell.CellData.SetType(CellType.Player);
			
			//inputHandler = newInstance.GetComponent<InputHandler>();
			PlayerInstance = newInstance.GetComponent<Player>();
			
			playerCamera.Follow = PlayerInstance.transform;
			PlayerInstance.SetHealth(playerHealth);
		}
		else
		{
			Debug.LogError("Not valid cell to spawn!");
		}
	}
	#endregion
	
	private void HandleActionExecuted(TurnAction action)
	{
		Debug.Log($"Action executed: {action.ActionType} - {action.Description}");
		actionExecuted = true;
	}
	
	
	
	
	private void onPlayerMove(HexCellComponent targetCell)
	{
		if (!TurnManager.CanExecuteAction(TurnActionType.Move))
			return;

		HexCellComponent playerCell = hexgrid.GetCellByType(CellType.Player);
		if (targetCell.CellData.CellGuiType == CellGuiType.ValidMoveRange)
		{
			OnPlayerMove.Invoke(targetCell);
			HexCellComponent[] oldNearbyCells = hexgrid.GetCellsInRange(playerCell, 1);
			for (int i = 0; i < oldNearbyCells.Length; i++)
			{
				oldNearbyCells[i].CellData.SetGuiType(CellGuiType.Empty);
			}

			playerCell.CellData.SetType(CellType.Empty);
			targetCell.CellData.SetType(CellType.Player);
			HexCellComponent[] newNearbyCells = hexgrid.GetCellsInRange(hexgrid.GetCellByType(CellType.Player), 1);
			for (int i = 0; i < newNearbyCells.Length; i++)
			{
				if (newNearbyCells[i].CellData.CellType == CellType.Empty)
				{
					newNearbyCells[i].CellData.SetGuiType(CellGuiType.ValidMoveRange);
				}
			}

			TurnManager.ExecuteAction(TurnActionType.Move,
				$"Moved from {playerCell.CellData.Coordinates} to {targetCell.CellData.Coordinates}");
		}
	}
	
	private IEnumerator _TurnBaseCoroutine()
	{
		//init player valid move range
		HexCellComponent[] newNearbyCells = hexgrid.GetCellsInRange(hexgrid.GetCellByType(CellType.Player), 1);
		for (int i = 0; i < newNearbyCells.Length; i++)
		{
			newNearbyCells[i].CellData.SetGuiType(CellGuiType.ValidMoveRange);
		}
    
		while (IsBattleStarted)
		{
			//TurnStart Action
			Debug.Log("New Turn Started");
			TurnManager.StartNewTurn();
        
			float remainingTime = initTurnDur;
			actionExecuted = false;

			// Invoke both versions of OnTurnStart
			OnTurnStart?.Invoke(initTurnDur);
			OnTurnStart?.Invoke();

			// Use Time.deltaTime for more precise timing
			while (!actionExecuted && remainingTime > 0)
			{
				remainingTime -= Time.deltaTime;
				yield return null;
			}
			
			//Add cards to hand at start of turn
			if (CardsManager.Instance.Hand.Count == 0)
			{
				for (int i = 0; i < handCardsSize; i++)
				{
					Card testCard = CardFactory.Instance.CreateCardFromList(AbilityDatabase, "1",
						AbilityDatabase.GetRandomAbilityFromList("1").id);
					CardsManager.Instance.AddCardToDeck(testCard);
					var (newDeck, newHand, drawnCard) = CardsManager.Instance.DrawCard();
				}
			}
			//TurnEnd Action
			TurnManager.EndTurn();
			yield return new WaitForSeconds(0.2f);
		}
	}

	#region ObjectCoordAPI

	public HexCellComponent GetPlayerCell()
	{
		return hexgrid.GetCellByType(CellType.Player);
	}

	#endregion
}



