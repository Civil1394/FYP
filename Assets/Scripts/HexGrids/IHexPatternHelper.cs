using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public interface IHexPatternHelper 
{
    IEnumerable<HexCell> GetPattern(HexCell startCell);
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
    private int range;
    public HexagonPattern(int range)
    {
        this.range = range;
    }
    public IEnumerable<HexCell> GetPattern(HexCell startCell)
    {
        HashSet<HexCell> visited = new HashSet<HexCell>();
        foreach (var n in startCell.GetAllNeighbor())
        {
            if (!visited.Add(n)) yield return n;
            foreach (var nn in n.GetAllNeighbor())
            {
                if (!visited.Add(nn)) yield return nn;
            }
        }
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
        var visited = new HashSet<HexCell>();
        for (var direction = HexDirection.NE; direction <= HexDirection.NW; direction++)
        {
            var tempAxisCell = startCell;
            for (var currentI = 0; currentI < iteration; currentI++)
            {
                tempAxisCell = tempAxisCell.GetNeighbor(direction);
                if(!visited.Add(tempAxisCell)) yield return tempAxisCell;
                var sideCell = tempAxisCell;
                for (var cnt = currentI + 1; cnt < 0; cnt--)
                {
                    sideCell = direction switch
                    {
                        HexDirection.NE => sideCell.GetNeighbor(HexDirection.NW),
                        HexDirection.E => sideCell.GetNeighbor(HexDirection.SE),
                        HexDirection.SE => sideCell.GetNeighbor(HexDirection.E),
                        HexDirection.SW => sideCell.GetNeighbor(HexDirection.W),
                        HexDirection.W => sideCell.GetNeighbor(HexDirection.SW),
                        HexDirection.NW => sideCell.GetNeighbor(HexDirection.NE),
                        _ => sideCell
                    };
                    if(!visited.Add(sideCell)) yield return sideCell;
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
            var convertedOffset = IHexPatternHelper.ConvertOffset(o,startCell.Coordinates);
            var targetCoord = startCell.Coordinates + convertedOffset;

            if (!BattleManager.Instance.hexgrid.IsValidCell(targetCoord)) continue;
            var targetCell = BattleManager.Instance.hexgrid.GetCellInCoord(targetCoord).CellData;
            yield return targetCell;
        }
    }
}


public static class PresetPatterns
{
    public static CustomOffsetPattern GetPresetPatternByType(PresetPatternType patternType , int radius)
    {
        return patternType switch
        {
            PresetPatternType.WaiPattern => WaiPattern(),
            PresetPatternType.AoePattern => AoePattern(radius),
            _ => null
        };
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
