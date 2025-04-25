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
        switch (color) {  case AbilityColorType.Red: return HexToColor("#FF3333"); // Bright Red
                        case AbilityColorType.Green: return HexToColor("#0ADD08"); // Vibrant Green
                        case AbilityColorType.Teal: return HexToColor("#02d9d9"); // True Teal
                        case AbilityColorType.Blue: return HexToColor("#265cff"); // Royal Blue
                        case AbilityColorType.Purple: return HexToColor("#a422e6"); // Rich Purple
                        case AbilityColorType.Black: return HexToColor("#333333"); // Dark Gray (not white)
                        default: return Color.white; // Default fallback color
                        }
    }
    public static Color DarkenColor(Color color, float darkenFactor = 0.8f)
    {
        return new Color(
            color.r * darkenFactor,
            color.g * darkenFactor,
            color.b * darkenFactor,
            color.a * 0.6f
        );
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