using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

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
            
            HexCell targetCell = BattleManager.Instance.hexgrid.GetCellInCoord(targetCoord).CellData;
            if (targetCell != null)
                yield return targetCell;
        }
    }
    private Vector3Int OffsetToAxial(Vector3Int offset)
    {
        int q = offset.x - (offset.z - (offset.z & 1)) / 2;
        return new Vector3Int(q, 0, offset.z);
    }

    private Vector3Int AxialToOffset(Vector3Int axial)
    {
        int x = axial.x + (axial.z - (axial.z & 1)) / 2;
        return new Vector3Int(x, 0, axial.z);
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