using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BlastBehavior : MonoBehaviour
{
	protected GameObject blastVFX;
	protected int width;
	protected HexCellComponent casterCell;
	protected HexDirection castingDirection;
	protected BlastParameter parameter;

	public virtual void Init(GameObject blastVFXPrefab,int width, HexDirection castingDirection,HexCellComponent casterCell)
	{
		this.blastVFX = blastVFXPrefab;
		this.width = width;
		this.casterCell = casterCell;
		this.castingDirection = castingDirection;
	}
	public abstract void UpdateBehavior();


}

public class LinearBlastBehavior : BlastBehavior
{
	private HexCellComponent[] cellsInStep;

	public override void Init(GameObject blastVFXPrefab, int width, HexDirection castingDirection, HexCellComponent casterCell)
	{
		base.Init(blastVFXPrefab, width, castingDirection, casterCell);
		//init cells in step 1
		cellsInStep = BattleManager.Instance.hexgrid.GetCellsByDirection(casterCell,
			HexDirectionHelper.GetDirectionsAround(castingDirection, width));
	}

	public override void UpdateBehavior()
	{
		for (int i = 0; i < width; i++)
		{
			if (cellsInStep[i] != null)
			{
				Instantiate(blastVFX,cellsInStep[i].transform.position,Quaternion.Euler(-90,0,0));
				var c = cellsInStep[i].CellData.GetNeighbor(castingDirection);
				if (c.CellType != CellType.Invalid)
					cellsInStep[i] = c.ParentComponent;
				else cellsInStep[i] = null;
			}
			else
			{
				Debug.LogWarning(i + " is blocked");
			}
			
		}
		
	}
	
}