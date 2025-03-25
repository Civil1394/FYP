using System.Collections.Generic;

[System.Serializable]
public class HexagonPattern : HexPatternBase
{
    public int range;
    // public HexagonPattern(){}
    // public HexagonPattern(int range)
    // {
    //     this.range = range;
    // }
    public override IEnumerable<HexCell> GetPattern(HexCell startCell)
    {
        var hccArray = BattleManager.Instance.hexgrid.GetCellsInRange(startCell.ParentComponent, range);
        foreach (var c in hccArray)
        {
            yield return c.CellData;
        }
    }
}