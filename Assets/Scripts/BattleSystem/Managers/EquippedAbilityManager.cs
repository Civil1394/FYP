using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class EquippedAbilityManager 
{

	private static List<AbilityData> equippedAbilities = new List<AbilityData>();
	private static AbilityDatabase currentDatabase;
	
	public static List<AbilityData> EquippedAbilities => equippedAbilities;
	
	public static void InitAbiltiyDatabase(AbilityDatabase abilityDatabase)
	{
		currentDatabase = abilityDatabase;
		equippedAbilities.Clear();
	}

	public static void InitEquippedAbilities(int count)
	{
		equippedAbilities.Clear();
		for (int i = 0; i < count; i++)
		{
			equippedAbilities.Add(CreateAbilityInstance());
		}
	}
	public static AbilityData CreateAbilityInstance()
	{
		var bp = currentDatabase.GetRandomAbilityFromList("1");
		var newAbilityInstance = bp.Create(bp,true);
		if (newAbilityInstance == null)
		{
			Debug.LogError("AbilityData is null,fail to create ability instance");
		}
		equippedAbilities.Add(newAbilityInstance);
		return newAbilityInstance;
	}

	public static void RemoveAndReplaceAbilityInDirection(HexDirection direction)
	{
		if (equippedAbilities.Count == 0)
		{
			Debug.LogError("Ability list is empty");
			return;
		}
    
		int index = (int)direction;
		if (index >= 0 && index < equippedAbilities.Count)
		{
			// Remove the ability at the specified direction
			equippedAbilities.RemoveAt(index);
        
			// Create a new ability and insert it at the same position
			var bp = currentDatabase.GetRandomAbilityFromList("1");
			var newAbilityInstance = bp.Create(bp, true);
        
			if (newAbilityInstance == null)
			{
				Debug.LogError("AbilityData is null, fail to create ability instance");
				return;
			}
        
			// Insert the new ability at the same index
			equippedAbilities.Insert(index, newAbilityInstance);
		}
	}
	public static void RemoveAbilityInDirection(HexDirection direction)
	{
		if (equippedAbilities.Count == 0)
		{
			Debug.LogError("Ability list is empty");
			return;
		}
		equippedAbilities.RemoveAt((int)direction);
	}

	public static bool CheckAnyEmptySlotInEquippedAbilities()
	{

		if(equippedAbilities.Count <GameConstants.AbilitySlotCount) return true;
		
		return false;
	}
	public static AbilityData GetEquippedAbilityData(int index)
	{
		if (index >= 0 && index < equippedAbilities.Count)
			return equippedAbilities[index];
		else
		{
			Debug.LogError($"Invalid ability index: {index}");
			return null;
		}
	}


	
}