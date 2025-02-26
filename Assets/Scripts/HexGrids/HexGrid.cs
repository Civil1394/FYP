using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HexGrid 
{

    private Dictionary<Vector3Int, HexCellComponent> cells = new Dictionary<Vector3Int, HexCellComponent>();
    public float CellDistance;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Dictionary<HexCellComponent,int> PlayerSixDirCellsSet = new Dictionary<HexCellComponent, int>();
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
    
    public bool HasCell(Vector3Int coordinates)
    {
        return cells.ContainsKey(coordinates);
    }

    public bool IsValidCell(Vector3Int coordinates)
    {
        return cells.ContainsKey(coordinates) && GetCellInCoord(coordinates).CellData.CellType != CellType.Invalid;
    }
    public bool IsValidCell(Vector2Int coord)
    {
        Vector3Int coordinates = new Vector3Int(coord.x, 0, coord.y);
        return cells.ContainsKey(coordinates) && GetCellInCoord(coordinates).CellData.CellType != CellType.Invalid;
    }

    #region Coordiantes Utilities

    public HexCellComponent GetCellInCoord(Vector3Int coordinates)
    {
        return cells.TryGetValue(coordinates, out HexCellComponent cell) ? cell : null;
    }
    public HexCellComponent GetCellInCoordVector2(Vector2Int coordinates)
    {
        Vector3Int coord = new Vector3Int(coordinates.x,  0, coordinates.y);
        return cells.TryGetValue(coord, out HexCellComponent cell) ? cell : null;
    }
    public HexCellComponent GetNearestAvailableCellByWorldPosition(Vector3 worldPos)
    {
        Ray r = new Ray(worldPos, Vector3.down);
        RaycastHit h;
        HexCellComponent hcc;
        if (Physics.Raycast(r, out h, LayerMask.NameToLayer("Cell")))
        {
            hcc = h.transform.gameObject.GetComponent<HexCellComponent>();
            return GetNearestCellOfType(hcc, CellType.Empty);
        }
        return null;
    }

    public float GetGridDistance(HexCellComponent cell1, HexCellComponent cell2)
    {
        return Vector3.Distance(cell1.CellData.Coordinates, cell2.CellData.Coordinates);
    }
    #endregion

    #region Range Utilities

    
    public HexCellComponent[] GetCellsInRange(HexCellComponent pivotCell, int range)
    {
        HashSet<Vector3Int> visitedCoordinates = new HashSet<Vector3Int>();
        Queue<(HexCellComponent, int)> queue = new Queue<(HexCellComponent, int)>();
        List<HexCellComponent> cellsInRange = new List<HexCellComponent>();
    
        queue.Enqueue((pivotCell, 0));
        visitedCoordinates.Add(pivotCell.CellData.Coordinates);

        while (queue.Count > 0)
        {
            var (currentCell, currentDistance) = queue.Dequeue();

            if (currentDistance > range)
            {
                continue;
            }

            cellsInRange.Add(currentCell);

            if (currentDistance < range)
            {
                foreach (var neighbor in currentCell.CellData.GetAllNeighbor())
                {
                    if (neighbor != null && !visitedCoordinates.Contains(neighbor.Coordinates))
                    {
                        visitedCoordinates.Add(neighbor.Coordinates);
                        queue.Enqueue((GetCellInCoord(neighbor.Coordinates), currentDistance + 1));
                    }
                }
            }
        }

        return cellsInRange.ToArray();
    }

    
   
    public HexCellComponent[] GetCellsInRangeByType(HexCellComponent pivotCell, int range, CellType cellType)
    {
        return GetCellsInRange(pivotCell, range)
            .Where(cell => cell.CellData.CellType == cellType)
            .ToArray();
    }
    
    public bool CheckCellInRange(HexCellComponent pivotCell, HexCellComponent cellToCheck, int range)
    {
        return CheckCellsInRange(pivotCell, new[] { cellToCheck }, range);
    }
    public bool CheckCellsInRange(HexCellComponent pivotCell, HexCellComponent[] cellsToCheck, int range)
    {
        HashSet<Vector3Int> visitedCoordinates = new HashSet<Vector3Int>();
        Queue<(HexCellComponent, int)> queue = new Queue<(HexCellComponent, int)>();
    
        queue.Enqueue((pivotCell, 0));
        visitedCoordinates.Add(pivotCell.CellData.Coordinates);

        while (queue.Count > 0)
        {
            var (currentCell, currentDistance) = queue.Dequeue();

            if (currentDistance > range)
            {
                continue;
            }

            foreach (var cellToCheck in cellsToCheck)
            {
                if (currentCell.CellData.Coordinates == cellToCheck.CellData.Coordinates)
                {
                    cellsToCheck = cellsToCheck.Where(c => c != cellToCheck).ToArray();
                    if (cellsToCheck.Length == 0)
                    {
                        return true; // All cells are within range
                    }
                    break;
                }
            }

            if (currentDistance < range)
            {
                foreach (var neighbor in currentCell.CellData.GetAllNeighbor())
                {
                    if (neighbor != null && !visitedCoordinates.Contains(neighbor.Coordinates))
                    {
                        visitedCoordinates.Add(neighbor.Coordinates);
                        queue.Enqueue((GetCellInCoord(neighbor.Coordinates), currentDistance + 1));
                    }
                }
            }
        }

        return false; // Not all cells are within range
    }
    #endregion
    
    #region Direction Utilities

    public HexCellComponent GetCellByDirection(HexCellComponent pivotCell, HexDirection direction)
    {
        if (pivotCell.CellData.GetNeighbor(direction) == null)
        {
            return null;
        }
            
        return cells.TryGetValue(pivotCell.CellData.GetNeighbor(direction).Coordinates, out HexCellComponent neighbor) ? neighbor : null;
    }

    public HexDirection CheckNeigborCellDirection(HexCellComponent pivotCell, HexCellComponent neigborCell)
    {
        for (int i = 0; i < pivotCell.CellData.Neighbors.Length; i++)
        {
            if (pivotCell.CellData.Neighbors[i] == neigborCell.CellData)
            {
                return (HexDirection)i;
            }

        }return HexDirection.NONE;
    }
    public HexCellComponent GetAvailableCellByWorldDirection(HexCellComponent pivotCell, Vector3 worldDir, int cell = 1)
    {
        worldDir.Normalize();
        HexCellComponent nearestCell = GetNearestAvailableCellByWorldPosition(
            pivotCell.transform.position + worldDir * (cell * CellDistance) + Vector3.up
        );
        return nearestCell;
    }
    public HexDirection GetHexDirectionBy2Cell(HexCellComponent pivotCell, HexCellComponent endCell)
    {
        Vector3 originDir = (endCell.transform.position - pivotCell.transform.position).normalized;
        for (int i = 0; i < pivotCell.CellData.Neighbors.Length; i++)
        {
            Vector3 tempDir = (pivotCell.CellData.Neighbors[i].ParentComponent.transform.position - pivotCell.transform.position).normalized;
            if (tempDir == originDir)
            {
                return (HexDirection)i;
            }
        }
        return HexDirection.NONE;
    }
    #endregion

    #region Type Utilities

    public HexCellComponent[] GetCellsByType(CellType cellType)
    {
        return cells.Values
            .Where(cell => cell.CellData.CellType == cellType)
            .ToArray();
    }
    public HexCellComponent GetCellByType(CellType cellType)
    {
        return cells.Values
            .FirstOrDefault(cell => cell.CellData.CellType == cellType);
    }
    
    public HexCellComponent GetNearestCellOfType(HexCellComponent pivotCell, CellType cellType)
    {
        HashSet<HexCellComponent> visitedList = new HashSet<HexCellComponent>();
        Queue<(HexCellComponent, int)> queue = new Queue<(HexCellComponent, int)>();
        List<HexCellComponent> cellsInRange = new List<HexCellComponent>();

        queue.Enqueue((pivotCell, 0));
        visitedList.Add(pivotCell);

        while (queue.Count > 0)
        {
            var (currentCell, currentDistance) = queue.Dequeue();

            cellsInRange.Add(currentCell);

            foreach (var neighbor in currentCell.CellData.GetAllNeighbor())
            {
                if (currentCell.CellData.CellType == cellType)
                {
                    return currentCell;
                }
                if (neighbor != null && !visitedList.Contains(neighbor.ParentComponent))
                {
                    visitedList.Add(neighbor.ParentComponent);
                    queue.Enqueue((GetCellInCoord(neighbor.Coordinates), currentDistance + 1));
                }
            }
        }
        return null;
    }
    
    #endregion

    //EnemyAttackStrategy logic use
    public void UpdatePlayerSixDirCellsSet()
    {
        PlayerSixDirCellsSet.Clear();
        HexCellComponent playerCell = GetCellByType(CellType.Player);
        for (HexDirection direction = HexDirection.NE; direction <= HexDirection.NW; direction++)
        {
            HexCellComponent temp = playerCell;
            int tempDis = 1;
            while (temp != null)
            {
                var c = GetCellByDirection(temp, direction);

                if (c == null || c.CellData.CellType == CellType.Invalid)
                    break;
                
                PlayerSixDirCellsSet.Add(c, tempDis++);
                temp = c;
            }
        }
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