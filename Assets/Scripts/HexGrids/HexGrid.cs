using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine.Serialization;

public class HexGrid 
{
   
    private Dictionary<Vector3Int, HexCellComponent> cells = new Dictionary<Vector3Int, HexCellComponent>();
    
    public int Width { get; private set; }
    public int Height { get; private set; }

    public HexGrid()
    {
    }

    public void AddCell(HexCellComponent cell)
    {
        if (!cells.ContainsKey(cell.CellData.Coordinates))
        {
            cells[cell.CellData.Coordinates] = cell;
            SetupNeighbors(cell);
        }
    }

    public HexCellComponent GetCell(Vector3Int coordinates)
    {
        return cells.TryGetValue(coordinates, out HexCellComponent cell) ? cell : null;
    }

    private void SetupNeighbors(HexCellComponent cell)
    {
        for (HexDirection direction = HexDirection.NE; direction <= HexDirection.NW; direction++)
        {
            Vector3Int neighborCoordinates = GetNeighborCoordinates(cell.CellData.Coordinates, direction);
            if (cells.TryGetValue(neighborCoordinates, out HexCellComponent neighbor))
            {
                cell.CellData.SetNeighbor(direction, neighbor.CellData);
            }
        }
    }

    private Vector3Int GetNeighborCoordinates(Vector3Int coordinates, HexDirection direction)
    {
        Vector3Int neighbor = coordinates;
        switch (direction)
        {
            case HexDirection.NE:
                neighbor.x += 1;
                neighbor.z += coordinates.x % 2 == 0 ? 0 : 1;
                break;
            case HexDirection.E:
                neighbor.x += 1;
                break;
            case HexDirection.SE:
                neighbor.x += 1;
                neighbor.z -= coordinates.x % 2 == 0 ? 1 : 0;
                break;
            case HexDirection.SW:
                neighbor.x -= 1;
                neighbor.z -= coordinates.x % 2 == 0 ? 1 : 0;
                break;
            case HexDirection.W:
                neighbor.x -= 1;
                break;
            case HexDirection.NW:
                neighbor.x -= 1;
                neighbor.z += coordinates.x % 2 == 0 ? 0 : 1;
                break;
        }
        return neighbor;
    }
}