using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : Singleton<BattleManager>
{
	[SerializeField] private AbilityDatabase abilityDatabase;
	public HexGrid hexgrid = new HexGrid();
	public void InitBattle()
	{
		if (abilityDatabase == null)
		{
			Debug.LogError("AbilityDatabase is not assigned to BattleManager!");
			return;
		}

		for (int i = 0; i < 3; i++)
		{
			Card testCard = CardFactory.Instance.CreateCardFromList(abilityDatabase,"1", abilityDatabase.GetRandomAbilityFromList("1").id);
			CardsManager.Instance.AddCardToDeck(testCard);
			CardsManager.Instance.DrawCard();
		}
		
	}

	private void Start()
	{
		InitBattle();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			// if (CardsManager.Instance.Hand.Count < 3)
			// {
			// 	Card testCard = CardFactory.Instance.CreateCardFromList(abilityDatabase,"1", abilityDatabase.GetRandomAbilityFromList("1").id);
			// 	CardsManager.Instance.AddCardToDeck(testCard);
			// }
			// var (newDeck, newHand, drawnCard) = CardsManager.Instance.DrawCard();
			// if (drawnCard != null)
			// {
			// 	Debug.Log($"Drew card: {drawnCard.Name}");
			// }
			// else
			// {
			// 	Debug.Log("No cards left in the deck");
			// }
			hexgrid.GetCell(new Vector3Int(15,0,19)).DebugTest();
			
		}
		
		
	}

	
}

