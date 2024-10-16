using UnityEngine;
using System;

[Serializable]
public class HexCell
{
    public int ID;
    public Vector3Int Coordinates;
    public HexCell[] Neighbors = new HexCell[6];

    // Serializable data container
    [Serializable]
    public class CellData
    {

    }

    public CellData Data = new CellData();

    // Helper method to get a neighbor in a specific direction
    public HexCell GetNeighbor(HexDirection direction)
    {
        return Neighbors[(int)direction];
    }

    // Helper method to set a neighbor in a specific direction
    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        Neighbors[(int)direction] = cell;
        cell.Neighbors[(int)direction.Opposite()] = this;
    }
}

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions
{
    public static HexDirection Opposite(this HexDirection direction)
    {
        return (HexDirection)(((int)direction + 3) % 6);
    }
}