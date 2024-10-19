using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : Singleton<BattleManager>
{
	public AbilityDatabase AbilityDatabase;
	public HexGrid hexgrid = new HexGrid() ;
	public void InitBattle()
	{
		if (AbilityDatabase == null)
		{
			Debug.LogError("AbilityDatabase is not assigned to BattleManager!");
			return;
		}

		for (int i = 0; i < 3; i++)
		{
			Card testCard = CardFactory.Instance.CreateCardFromList(AbilityDatabase,"1", AbilityDatabase.GetRandomAbilityFromList("1").id);
			CardsManager.Instance.AddCardToDeck(testCard);
			CardsManager.Instance.DrawCard();
		}
		
		EnemyManager.Instance.InitEnemies();
	}

	private void Start()
	{
		InitBattle();
	}

	private void Update()
	{
		
		
		
	}

	
}

