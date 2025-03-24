using System.Collections.Generic;
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
[System.Serializable]
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
[System.Serializable]
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
[System.Serializable]
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
[System.Serializable]
public class CustomOffsetPattern: IHexPatternHelper
{
    public List<Vector3Int> Offsets;
    public CustomOffsetPattern(params Vector3Int[] offsets)
    {
        this.Offsets = offsets.ToList();
    }
    public CustomOffsetPattern(params Vector2Int[] offsets)
    {
        this.Offsets = new List<Vector3Int>();
        foreach (var o in offsets)
        {
            this.Offsets.Add(new Vector3Int(o.x, 0, o.y));
        }
    }
    public IEnumerable<HexCell> GetPattern(HexCell startCell)
    {
        //Vector3Int axialPos = OffsetToAxial(startCell.Coordinates);
        
        foreach (var o in Offsets)
        {
            var convertedOffset = IHexPatternHelper.ConvertOffset(o,startCell.Coordinates);
            var targetCoord = startCell.Coordinates + convertedOffset;

            if (!BattleManager.Instance.hexgrid.IsValidCell(targetCoord)) continue;
            var targetCell = BattleManager.Instance.hexgrid.GetCellInCoord(targetCoord).CellData;
            yield return targetCell;
        }
    }
}
