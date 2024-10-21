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
	public HexGrid hexgrid = new HexGrid();

	[Header("Turn Related Ref")]
	[SerializeField] private float initTurnDur = 2f;

	public bool IsPlayerTurn;
	public bool IsInBattle = false;
	public Action<bool> OnTurnStart;
	public Action<HexCellComponent, HexCellComponent> OnPlayerMove;

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

		InitPlayer();
		EnemyManager.Instance.InitEnemies();

		OnPlayerMove += onPlayerMove;
		
		IsInBattle = true;
		StartCoroutine(_TurnBaseCoroutine());
	}

	private void Start()
	{
		InitBattle();
	}

	private void Update()
	{



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
		}
		else
		{
			Debug.LogError("Not valid cell to spawn!");
		}
	}
	
	private void onPlayerMove(HexCellComponent originCell, HexCellComponent targetCell)
	{
		HexCellComponent[] oldNearbyCells = hexgrid.GetCellsInRange(hexgrid.GetCellByType(CellType.Player), 1);
		for (int i = 0; i < oldNearbyCells.Length; i++)
		{
			oldNearbyCells[i].CellData.SetGuiType(CellGuiType.Empty);
		}
		
		originCell.CellData.SetType(CellType.Empty);
		targetCell.CellData.SetType(CellType.Player);
		HexCellComponent[] newNearbyCells = hexgrid.GetCellsInRange(hexgrid.GetCellByType(CellType.Player), 1);
		for (int i = 0; i < newNearbyCells.Length; i++)
		{
			newNearbyCells[i].CellData.SetGuiType(CellGuiType.ValidMoveRange);
		}
	}

	private IEnumerator _TurnBaseCoroutine()
	{
		HexCellComponent[] newNearbyCells = hexgrid.GetCellsInRange(hexgrid.GetCellByType(CellType.Player), 1);
		for (int i = 0; i < newNearbyCells.Length; i++)
		{
			newNearbyCells[i].CellData.SetGuiType(CellGuiType.ValidMoveRange);
		}
		while (IsInBattle)
		{
			OnTurnStart.Invoke(true);
			
			yield return new WaitForSeconds(initTurnDur);
		}
	}
}



