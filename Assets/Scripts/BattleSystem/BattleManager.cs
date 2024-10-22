using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;

public class BattleManager : Singleton<BattleManager>
{
	[Header("Player-related ref")] public AbilityDatabase AbilityDatabase;
	[SerializeField] private GameObject playerPrefab;
	[SerializeField] private Vector2Int playerSpawnCoord;
	[SerializeField] private CinemachineVirtualCamera playerCamera;
	[SerializeField] private InputHandler inputHandler;
	public HexGrid hexgrid = new HexGrid();

	[Header("Turn Related Ref")]
	[SerializeField] private float initTurnDur = 2f;
	
	public bool IsBattleStarted = false;
	public GenericAction OnTurnStart = new GenericAction();
	public Action<HexCellComponent> OnPlayerMove;

	private TurnManager turnManager;
	
	public void InitBattle()
	{
		if (AbilityDatabase == null)
		{
			Debug.LogError("AbilityDatabase is not assigned to BattleManager!");
			return;
		}

		for (int i = 0; i < 3; i++)
		{
			Card testCard = CardFactory.Instance.CreateCardFromList(AbilityDatabase, "1",
				AbilityDatabase.GetRandomAbilityFromList("1").id);
			CardsManager.Instance.AddCardToDeck(testCard);
			CardsManager.Instance.DrawCard();
		}
		
		
		//InitPlayer
		InitPlayer();
		inputHandler.OnClick.AddListener<HexCellComponent>(onPlayerMove);
		
		//InitEnemies
		EnemyManager.Instance.InitEnemies();
		
		//InitTurn
		turnManager = new TurnManager();
		turnManager.OnActionExecuted += HandleActionExecuted;
		IsBattleStarted = true;
		StartCoroutine(_TurnBaseCoroutine());
	}
	private void HandleActionExecuted(TurnAction action)
	{
		Debug.Log($"Action executed: {action.ActionType} - {action.Description}");
	}
	private void Start()
	{
		InitBattle();
	}

	private void OnDestroy()
	{
		inputHandler.OnClick.RemoveListener<HexCellComponent>(onPlayerMove);
	}

	private void InitPlayer()
	{
		HexCellComponent cell = hexgrid.GetCellInCoord(new Vector3Int(playerSpawnCoord.x, 0, playerSpawnCoord.y));
		if (cell.CellData.CellType == CellType.Empty)
		{
			GameObject newInstance = 
				Instantiate(playerPrefab, new Vector3(cell.transform.position.x,playerPrefab.transform.position.y,cell.transform.position.z)
					, quaternion.identity);
			playerCamera.Follow = newInstance.transform;
			//newInstance.currentCoord = cell.CellData.Coordinates;
			cell.CellData.SetType(CellType.Player);
			inputHandler = newInstance.GetComponent<InputHandler>();
		}
		else
		{
			Debug.LogError("Not valid cell to spawn!");
		}
	}
	
	private void onPlayerMove(HexCellComponent targetCell)
	{
		if (!turnManager.CanExecuteAction(TurnActionType.Move))
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

			turnManager.ExecuteAction(TurnActionType.Move,
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
			Debug.Log("New Turn Started");
			turnManager.StartNewTurn();
			OnTurnStart?.Invoke(initTurnDur);
            OnTurnStart?.Invoke();
			yield return new WaitForSeconds(initTurnDur);
            
			turnManager.EndTurn();
		}
	}


}



