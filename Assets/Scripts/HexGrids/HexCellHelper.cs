using UnityEngine;
using System;

public enum HexDirection
{
    //NE = 0, E = 1, SE = 2, SW = 3, W = 4, NW = 5 , NONE = 6
    NW = 0, W = 1, SW = 2, SE = 3, E = 4, NE = 5, NONE = 6
}

public static class HexDirectionExtensions
{
    public static HexDirection Opposite(this HexDirection direction)
    {
        return (HexDirection)(((int)direction + 3) % 6);
    }
}

public enum CellType
{
    Empty = 0,
    Player = 1,
    Enemy = 2,
    Invalid = 3,
   
}

public enum CellGuiType
{
    Empty = 0,
    ValidMoveCell = 1,
    ValidAttackCell = 2
}