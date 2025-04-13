using UnityEngine;
using System;

public enum HexDirection
{
    //NE = 0, E = 1, SE = 2, SW = 3, W = 4, NW = 5 , NONE = 6
    NW = 0, W = 1, SW = 2, SE = 3, E = 4, NE = 5, NONE = 6
}

public static class HexDirectionHelper
{
    public static HexDirection Opposite(this HexDirection direction)
    {
        return (HexDirection)(((int)direction + 3) % 6);
    }

    
    public static HexDirection[] GetDirectionsAround(HexDirection centerDirection, int width)
    {
        
        HexDirection[] directions = new HexDirection[width];

        if (width >= 6)
        {
            for (int i = 0; i < 6; i++)
            {
                directions[i] = (HexDirection)i;
            }
        }
        if (width % 2 == 0)
        {
            Debug.LogError(width + " is not a odd number or > 1");
            return directions;
        }
        
        int k = (width - 1) / 2;
        int start = ((int)centerDirection - k + 6) % 6;
        for(int i = 0; i < width; i++)
        {
            directions[i] = (HexDirection)((start + i) % 6);
        }
        
        return directions;
    }

    public static float DeltaDegreeRotationForProjectile(HexDirection direction)
    {
        switch (direction)
        {
            case HexDirection.NW:
                return -120;
            case HexDirection.W:
                return -180;
            case HexDirection.SW:
                return 120;
            case HexDirection.SE:
                return 60;
            case HexDirection.E:
                return 0;
            case HexDirection.NE:
                return -60;
        }
        return 0;
    }
}

public enum CellType
{
    Empty = 0,
    Player = 1,
    Enemy = 2,
    Chest = 3,
    Invalid = 4,
   
}

public enum CellActionType
{
    Empty = 0,
    ValidMoveCell = 1,
    ValidAttackCell = 2,
    Chest = 3
}