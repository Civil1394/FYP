using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Resources;
using Microsoft.Unity.VisualStudio.Editor;

public interface IHexPatternHelper 
{
    IEnumerable<HexCell> GetPattern(HexCell startCell);
}
public class LinePattern : IHexPatternHelper
{
    private int range;
    private HexDirection[] dir;
    public LinePattern(int range, params HexDirection[] dir)
    {
        this.range = range;
        this.dir = dir;
    }
    public IEnumerable<HexCell> GetPattern(HexCell startCell)
    {
        foreach (var d in dir)
        {
            for (var i = 0; i < range; i++)
            {
                HexCell targetCell = startCell.GetNeighbor(d);
                if (targetCell != null)
                    yield return targetCell;
            }
        }
    }
}
public class HexagonPattern : IHexPatternHelper
{
    public IEnumerable<HexCell> GetPattern(HexCell startCell)
    {
        throw new NotImplementedException();
    }
}
public class TrianglePattern : IHexPatternHelper
{
    private int iteration;
    private bool isUpward;
    public TrianglePattern(int iteration, bool isUpward = true)
    {
        this.iteration = iteration;
        this.isUpward = isUpward;
    }
    public IEnumerable<HexCell> GetPattern(HexCell startCell)
    {
        HashSet<HexCell> visitedCells = new HashSet<HexCell>();
        for (HexDirection direction = HexDirection.NE; direction <= HexDirection.NW; direction++)
        {
            HexCell tempAxisCell = startCell;
            for (int currentI = 0; currentI < iteration; currentI++)
            {
                tempAxisCell = tempAxisCell.GetNeighbor(direction);
                HexCell sideCell = tempAxisCell;
                for (int cnt = currentI + 1; cnt < 0; cnt--)
                {
                    switch (direction)
                    {
                        case HexDirection.NE:
                            sideCell = sideCell.GetNeighbor(HexDirection.NW);
                            break;
                        case HexDirection.E:
                            sideCell = sideCell.GetNeighbor(HexDirection.SE);
                            break;
                        case HexDirection.SE:
                            sideCell = sideCell.GetNeighbor(HexDirection.E);
                            break;
                        case HexDirection.SW:
                            sideCell = sideCell.GetNeighbor(HexDirection.W);
                            break;
                        case HexDirection.W:
                            sideCell = sideCell.GetNeighbor(HexDirection.SW);
                            break;
                        case HexDirection.NW:
                            sideCell = sideCell.GetNeighbor(HexDirection.NE);
                            break;
                    }

                    if (!visitedCells.Contains(sideCell))
                    {
                        visitedCells.Add(sideCell);
                        yield return sideCell;
                    }
                }
                if (!visitedCells.Contains(tempAxisCell))
                {
                    visitedCells.Add(tempAxisCell);
                    yield return tempAxisCell;
                }
            }
        }
    }
}
public class VertexDirectionPattern: IHexPatternHelper
{
    private int numOfDir;
    private int range;
    private HexVertexDirection[] dir;
    public VertexDirectionPattern(int numOfDir, int range, params HexVertexDirection[] dir)
    {
        this.numOfDir = numOfDir;
        this.range = range;
        this.dir = dir;
    }
    public IEnumerable<HexCell> GetPattern(HexCell startCell)
    { 
        //loop through switch on vertex direction, implement some helper function for difference in odd and even row
        throw new NotImplementedException();
    }
}
public class CustomOffsetPattern: IHexPatternHelper
{
    private readonly List<Vector3Int> offsets;
    public CustomOffsetPattern(params Vector3Int[] offsets)
    {
        this.offsets = offsets.ToList();
    }
    public IEnumerable<HexCell> GetPattern(HexCell startCell)
    {
        //Vector3Int axialPos = OffsetToAxial(startCell.Coordinates);
        
        foreach (var o in offsets)
        {
            Vector3Int convertedOffset = ConvertOffset(o,startCell.Coordinates);
            Vector3Int targetCoord = startCell.Coordinates + convertedOffset;
            // Vector3Int targetAxial = axialPos + o;
            // Vector3Int targetOffset = AxialToOffset(targetAxial);
            
            HexCell targetCell = BattleManager.Instance.hexgrid.GetCellInCoord(targetCoord)?.CellData;
            if (targetCell != null)
                yield return targetCell;
        }
    }

    public static Vector3Int ConvertOffset(Vector3Int originalOffset, Vector3Int currentPosition)
    {
        if (originalOffset.z % 2 == 0)
        {
            return originalOffset;
        }
        if (currentPosition.z % 2 != 0)
        {
            return originalOffset;
        }
        return new Vector3Int(
            originalOffset.x - 1,
            0,
            originalOffset.z
        );
    }
}


public static class PresetPatterns
{
    public static CustomOffsetPattern GetPresetPatternByType(PresetPatternType patternType , int radius)
    {
        switch (patternType)
        {
            case PresetPatternType.WaiPattern:
                return WaiPattern();
            break;
            case PresetPatternType.AoePattern:
                return AoePattern(radius);
        }

        return null;
    }
     public static CustomOffsetPattern WaiPattern()
    {
        return new CustomOffsetPattern(
            new Vector3Int(0, 0, -3),
            new Vector3Int(1, 0, -3),
            new Vector3Int(0, 0, -4),
            new Vector3Int(-2, 0, 1),
            new Vector3Int(-2, 0, 2),
            new Vector3Int(-3, 0, 2),
            new Vector3Int(3, 0, 1),
            new Vector3Int(2, 0, 2),
            new Vector3Int(3, 0, 2)
        );
    }


    public static CustomOffsetPattern AoePattern(int radius)
    {
        // Calculate the total number of cells in the pattern
        int totalCells = radius * 6;
        Vector3Int[] cells = new Vector3Int[totalCells];

        int index = 0;

        // Generate cells based on the 6 directions in a hexagonal grid
        for (int r = 1; r <= radius; r++)
        {
            cells[index++] = new Vector3Int(r, 0, 0);    // +X direction
            cells[index++] = new Vector3Int(-r, 0, 0);   // -X direction
            cells[index++] = new Vector3Int(-r, 0, r);    // +Z direction
            cells[index++] = new Vector3Int(-r, 0, -r);   // -Z direction
            cells[index++] = new Vector3Int(r, 0, r);   // +X, -Z diagonal
            cells[index++] = new Vector3Int(r, 0, -r);   // -X, +Z diagonal
        }
        
        return new CustomOffsetPattern(cells);
    }


}


public enum PresetPatternType
{
    WaiPattern = 0,
    AoePattern = 1
        
}
