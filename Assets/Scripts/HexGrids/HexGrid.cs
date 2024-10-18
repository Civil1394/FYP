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
        //W direction
        if(cell.CellData.Coordinates.x > 0)
            if (cells.TryGetValue(cell.CellData.Coordinates + Vector3Int.left, out HexCellComponent wNeighbor))
            {
                cell.CellData.SetNeighbor(HexDirection.W, wNeighbor.CellData);
            }

        int columnFlag = (cell.CellData.Coordinates.z % 2 == 0) ? 1 : 0;
        
        if (cell.CellData.Coordinates.z > 0)
        {
            //NW
            Vector3Int nwNeighborCoord = new Vector3Int( cell.CellData.Coordinates.x -columnFlag, 0, cell.CellData.Coordinates .z -1);
            if (cells.TryGetValue(nwNeighborCoord, out HexCellComponent nwNeighbor))
            {
                cell.CellData.SetNeighbor(HexDirection.NW, nwNeighbor.CellData);
            }
            //NE
            Vector3Int neNeighborCoord = new Vector3Int(cell.CellData.Coordinates.x - columnFlag + 1, 0, cell.CellData.Coordinates.z - 1);
            if (cells.TryGetValue(neNeighborCoord, out HexCellComponent neNeighbor))
            {
                cell.CellData.SetNeighbor(HexDirection.NE, neNeighbor.CellData);
            }
        }
       
    }
    
}