using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public interface IHexPatternHelper 
{
    IEnumerable<HexCell> GetPattern(HexCell startCell);
}

public abstract class HexPatternBase : ScriptableObject, IHexPatternHelper
{
    public Vector3Int ConvertOffset(Vector3Int originalOffset, Vector3Int currentPosition)
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

    public abstract IEnumerable<HexCell> GetPattern(HexCell startCell);
}