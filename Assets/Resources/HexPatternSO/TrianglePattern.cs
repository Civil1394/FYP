using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrianglePattern : HexPatternBase
{
    public int iteration;
    public bool isUpward;
    // public TrianglePattern(){}
    // public TrianglePattern(int iteration, bool isUpward = true)
    // {
    //     this.iteration = iteration;
    //     this.isUpward = isUpward;
    // }

    public override IEnumerable<HexCell> GetPattern(HexCell startCell)
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
                    Debug.Log(this.GetType()+" "+sideCell.Coordinates);
                }
            }
        }
    }
}