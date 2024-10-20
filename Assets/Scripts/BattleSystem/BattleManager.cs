using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class BattleManager : Singleton<BattleManager>
{
	[Header("Player-related ref")] public AbilityDatabase AbilityDatabase;
	[SerializeField] private GameObject playerPrefab;
	[SerializeField] private Vector2Int playerSpawnCoord;

	public HexGrid hexgrid = new HexGrid();

	[Header("Turn Related Ref")] [SerializeField]
	private float initTurnDur;

	public bool IsPlayerTurn;
	public bool IsBattleMode = false;
	public Action<bool> OnTurnSwitch;

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

		IsBattleMode = true;
		//StartCoroutine(_TurnBaseCoroutine());
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
		HexCellComponent cell = hexgrid.GetCell(new Vector3Int(playerSpawnCoord.x, 0, playerSpawnCoord.y));
		if (cell.CellData.CellType == CellType.Empty)
		{
			GameObject newInstance = 
				Instantiate(playerPrefab, new Vector3(cell.transform.position.x,playerPrefab.transform.position.y,cell.transform.position.z)
					, quaternion.identity);
			//newInstance.currentCoord = cell.CellData.Coordinates;
			cell.CellData.CellType = CellType.Player;
		}
		else
		{
			Debug.LogError("Not valid cell to spawn!");
		}
	}



	private IEnumerator _TurnBaseCoroutine()
	{
		while (IsBattleMode)
		{
			yield return new WaitForSeconds(initTurnDur);
		}
	}
}



