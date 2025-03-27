using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public partial class LinePattern : HexPatternBase
{
    public int range;
    public HexDirection[] dir;

    // public LinePattern(){}
    // public LinePattern(int range, params HexDirection[] dir)
    // {
    //     this.range = range;
    //     this.dir = dir;
    // }
    public override IEnumerable<HexCell> GetPattern(HexCell startCell)
    {
        foreach(var d in dir)
        {
            HexCell temp = startCell;
            for (var i = 0; i < range; i++)
            {
                temp = temp.GetNeighbor(d);
                yield return temp;
            }
        }
        //foreach (var d in dir)
        //{
        //    HexCell targetCell = startCell.GetNeighbor(d);
        //    yield return targetCell;
        //    for (var i = 0; i < range; i++)
        //    {
        //        targetCell = targetCell.GetNeighbor(d);
        //        if (targetCell != null)
        //            yield return targetCell;
        //        
        //    }
        //}
    }
}