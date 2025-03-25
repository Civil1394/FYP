using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomOffsetPattern: HexPatternBase
{
    public List<Vector3Int> Offsets;
    // public CustomOffsetPattern(){}
    // public CustomOffsetPattern(params Vector3Int[] offsets)
    // {
    //     this.Offsets = offsets.ToList();
    // }
    // public CustomOffsetPattern(params Vector2Int[] offsets)
    // {
    //     this.Offsets = new List<Vector3Int>();
    //     foreach (var o in offsets)
    //     {
    //         this.Offsets.Add(new Vector3Int(o.x, 0, o.y));
    //     }
    // }
    public override IEnumerable<HexCell> GetPattern(HexCell startCell)
    {
        //Vector3Int axialPos = OffsetToAxial(startCell.Coordinates);
        
        foreach (var o in Offsets)
        {
            var convertedOffset = ConvertOffset(o,startCell.Coordinates);
            var targetCoord = startCell.Coordinates + convertedOffset;

            if (!BattleManager.Instance.hexgrid.IsValidCell(targetCoord)) continue;
            var targetCell = BattleManager.Instance.hexgrid.GetCellInCoord(targetCoord).CellData;
            yield return targetCell;
            Debug.Log(this.GetType()+" "+targetCell.Coordinates);
        }
    }
}