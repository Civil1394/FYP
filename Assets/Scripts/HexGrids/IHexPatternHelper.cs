using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public interface IHexPatternHelper 
{
    IEnumerable<HexCell> GetPossibleMoves
    (
        HexCell startCell,
        Func<Vector3Int, HexCell> getCellByCoordinate,
        Func<HexCell, bool> isValidCell
    );
}
public class LinePattern : IHexPatternHelper
{
    private int numOfDir;
    private int range;
    private HexDirection[] dir;
    public LinePattern(int numOfDir, int range, params HexDirection[] dir)
    {
        this.numOfDir = numOfDir;
        this.range = range;
        this.dir = dir;
    }
    public IEnumerable<HexCell> GetPossibleMoves(HexCell startCell, Func<Vector3Int, HexCell> getCellByCoordinate, Func<HexCell, bool> isValidCell)
    {
        foreach (var d in dir)
        {
            for (var i = 0; i < numOfDir; i++)
            {
                HexCell targetCell = startCell.GetNeighbor(d);
                if (targetCell != null && isValidCell(targetCell))
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
    public IEnumerable<HexCell> GetPossibleMoves(HexCell startCell, Func<Vector3Int, HexCell> getCellByCoordinate, Func<HexCell, bool> isValidCell)
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
    public IEnumerable<HexCell> GetPossibleMoves(HexCell startCell, Func<Vector3Int, HexCell> getCellByCoordinate, Func<HexCell, bool> isValidCell)
    {
        foreach (var o in offsets)
        {
            Vector3Int targetCoord = startCell.Coordinates + o;
            HexCell targetCell = getCellByCoordinate(targetCoord);
            if (targetCell != null && isValidCell(targetCell))
                yield return targetCell;
        }
    }
}