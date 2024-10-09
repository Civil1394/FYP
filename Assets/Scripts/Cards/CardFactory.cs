using UnityEngine;
using System.Collections;

public class CardFactory : Singleton<CardFactory>
{
	public Card CreateCardFromList(AbilityDatabase abilityDatabase,string listName, string abilityId)
	{
		var abilityData = abilityDatabase.GetSpecAbilityFromList(listName, abilityId);
		if (abilityData != null)
		{
			Card newCard = new Card();
			newCard.SetAbilityData(abilityData);
			return newCard;
		}
		return null;
	}
}

