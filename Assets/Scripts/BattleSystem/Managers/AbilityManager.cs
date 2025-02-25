using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AbilityManager 
{

	private static List<AbilityData> equippedAbilities = new List<AbilityData>();
	private static AbilityDatabase currentDatabase;
	
	public static List<AbilityData> EquippedAbilities => equippedAbilities;
	
	public static void InitBaseAbilities(AbilityDatabase abilityDatabase)
	{
		currentDatabase = abilityDatabase;
		equippedAbilities.Clear();
		
		for (int i = 0; i < GameConstants.AbilitySlotCount; i++)
		{
			var bp = currentDatabase.GetAbilityList("1")[i];
			var ability = bp.Create(bp,true);

			if (ability == null) return;
			equippedAbilities.Add(ability);
		}
	}


	public static AbilityData GetEquippedAbility(int index)
	{
		if (index >= 0 && index < equippedAbilities.Count)
			return equippedAbilities[index];
		else
		{
			Debug.LogError($"Invalid ability index: {index}");
			return null;
		}
	}


	public static void AddAbility(AbilityData ability)
	{
		if (equippedAbilities.Count + 1 > GameConstants.AbilitySlotCount)
		{
			Debug.LogError("Ability list is full, Exchange with olds");
			//TODO:Implement ability exchange screen
			return;
		}
		equippedAbilities.Add(ability);
	}

	public static void RemoveAbility(AbilityData ability)
	{
		if (equippedAbilities.Count == 0)
		{
			Debug.LogError("Ability list is empty");
			return;
		}
		equippedAbilities.Remove(ability);
	}
}