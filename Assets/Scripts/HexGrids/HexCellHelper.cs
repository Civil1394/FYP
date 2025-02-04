using UnityEngine;
using System;

public enum HexDirection
{
    NE = 0, E = 1, SE = 2, SW = 3, W = 4, NW = 5 , NONE = 6
}

public static class HexDirectionExtensions
{
    public static HexDirection Opposite(this HexDirection direction)
    {
        return (HexDirection)(((int)direction + 3) % 6);
    }
}

public enum HexVertexDirection
{
    N = 0, NE = 1, SE = 2, S = 3, SW = 4,NW = 5, NONE = 6
}
public static class HexVertexDirectionExtensions
{
    public static HexVertexDirection Opposite(this HexVertexDirection direction)
    {
        return (HexVertexDirection)(((int)direction + 3) % 6);
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
    ValidMoveRange = 1,
    ValidAttackRange = 2
}