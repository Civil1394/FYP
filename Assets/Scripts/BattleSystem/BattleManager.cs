using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : Singleton<BattleManager>
{
	[SerializeField] private AbilityDatabase abilityDatabase;
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
			Debug.Log($"Found Card: {testCard.Name}");
			CardsManager.Instance.AddCardToDeck(testCard);
		}
		
	}

	private void Start()
	{
		InitBattle();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			var (newDeck, newHand, drawnCard) = CardsManager.Instance.DrawCard();
			if (drawnCard != null)
			{
				Debug.Log($"Drew card: {drawnCard.Name}");
				drawnCard.TriggerCard();
			}
			else
			{
				Debug.Log("No cards left in the deck");
			}
			
		}
		
	}
}

