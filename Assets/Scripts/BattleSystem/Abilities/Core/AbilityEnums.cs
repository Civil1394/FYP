using UnityEngine;
using System.Collections;

public enum AbilityType
{
	Projectile = 0,
	ProjectileVolley = 1,
	ParabolaProjectile = 2,
	ExplosiveCharge = 5,
	LocationalProjectile = 6,
	Blast = 10,
	Dash = 20,
	Utility = 30
}

public enum AbilityCastType
{
	Auto_targeted,
	Direction_targeted,
	Location_targeted,
	Unit_targeted,
	Self_cast,
}
public enum AbilityTarget
{
	None,
	Player,
	Enemy,
	Environment
}

//Identify who cast the ability
public enum CasterType
{
	Player,
	Enemy,
	None
}

public enum AbilityColorType
{
	Maroon = 0,
	Green = 1,
	Teal = 2,
	Navy = 3,
	Purple = 4,
	Black = 5
}

public static class AbilityColorHelper
{
	public static Color GetAbilityColor(AbilityColorType color)
	{
		switch (color)
		{
			case AbilityColorType.Maroon:
				return HexToColor("#FF8080"); // Violet
			case AbilityColorType.Green:
				return HexToColor("#80FF80"); // Amber / Gold
			case AbilityColorType.Teal:
				return HexToColor("#80FFFF"); // Muted Cyan
			case AbilityColorType.Navy:
				return HexToColor("#8080FF"); // Muted Cyan
			case AbilityColorType.Purple:
				return HexToColor("#FF80FF"); // Muted Cyan
			case AbilityColorType.Black:
				return HexToColor("#FFFFFF"); // Muted Cyan
			default:
				return Color.white; // Default fallback color
		}
	}
    

	private static Color HexToColor(string hex)
	{
		Color color;
		if (ColorUtility.TryParseHtmlString(hex, out color))
		{
			return color;
		}
		return Color.white; // Fallback in case of an error
	}
}
