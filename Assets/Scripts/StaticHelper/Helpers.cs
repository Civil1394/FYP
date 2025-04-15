using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    
        
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0,45,0));

    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
    
    
    public static string GetUniqueID(object obj)
    {
        Type type = obj.GetType();
        return $"{type.Name}_{Guid.NewGuid().ToString("N")}";
    }

    public static string GetShortFormTimeType(TimeType timeType)
    {
        switch (timeType)
        {
            case TimeType.Boost:
                return "B";
            case TimeType.Slow:
                return "S";
            case TimeType.None:
                return "N";
        }
           return "Error";
    }
    
    
}

public static class GameConstants
{
    public const int AbilitySlotCount = 6;
}


public enum AbilityColorType
{
    Red = 0,
    Green = 1,
    Teal = 2,
    Blue = 3,
    Purple = 4,
    Black = 5
}

public static class AbilityColorHelper
{
    public static Color GetAbilityColor(AbilityColorType color)
    {
        switch (color)
        {
            case AbilityColorType.Red:
                return HexToColor("#FF8080"); // Violet
            case AbilityColorType.Green:
                return HexToColor("#80FF80"); // Amber / Gold
            case AbilityColorType.Teal:
                return HexToColor("#80FFFF"); // Muted Cyan
            case AbilityColorType.Blue:
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