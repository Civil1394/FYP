using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class NoiseSystem 
{
	public static List<Vector2Int> GetPositions(int count, int width, int height)
	{
		var positions = new List<Vector2Int>();
		var rand = new System.Random();
		var minDist = MathF.Sqrt((width * height) / (float)count);
		var attempts = 1000;
		while (positions.Count < count && attempts-- > 0)
		{
			var p = new Vector2Int(rand.Next(0, width), rand.Next(0, height));
			if (!BattleManager.Instance.hexgrid.IsValidCell(p)) continue;
			if (positions.All(pos => Vector2Int.Distance(pos, p) >= minDist))
				positions.Add(p);
		}
		return positions;
	}

}