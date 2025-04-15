using System;
using System.Collections.Generic;
using UnityEngine;

public static class AbilityColorTypeInitializer
{
	private static Dictionary<AbilityType, AbilityColorType> abilityColorMap;

	public static void InitAbilityColorType()
	{
		abilityColorMap = new Dictionary<AbilityType, AbilityColorType>();
		Array abilityTypes = Enum.GetValues(typeof(AbilityType));
		Array colorTypes = Enum.GetValues(typeof(AbilityColorType));

		System.Random random = new System.Random();

		foreach (AbilityType ability in abilityTypes)
		{
			AbilityColorType randomColor = (AbilityColorType)colorTypes.GetValue(random.Next(colorTypes.Length));
			abilityColorMap[ability] = randomColor;
		}
		//Debug use messages
		foreach (var entry in abilityColorMap)
		{
			Debug.Log($"Ability: {entry.Key}, Color: {entry.Value}");
		}
	}

	public static AbilityColorType GetAbilityColorType(AbilityType type)
	{
		return abilityColorMap[type];
	}
}