using System;
using UnityEngine;
using System.Collections;

public class Card : ICard
{
	public string Id { get; private set; }
	public string Name { get; private set; }
	public string Desc { get; private set; }
	public Suit Suit { get; private set; }
	public AbilityData AbilityData { get; private set; }
	
	public void SetSuit(Suit suit) => Suit = suit;
	public void SetAbilityData(AbilityData abilityData)
	{
		AbilityData = abilityData;
		Id = abilityData.id;
		Name = abilityData.title;
	}

	public void TriggerCard()
	{
		AbilityData.TriggerAbility();
	}

	
}



