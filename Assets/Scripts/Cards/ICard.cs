using UnityEngine;
using System.Collections;

public interface ICard
{
	string Id { get; }
	string Name { get; }
	Suit Suit { get; }
	AbilityData AbilityData { get; }

	void SetSuit(Suit suit);
	void SetAbilityData(AbilityData abilityData);
	void Cast();

}
	
	

