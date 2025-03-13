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
	private List<HexCellComponent> cellsInLastStep = new List<HexCellComponent>();
	

	public override void UpdateBehavior()
	{
		for (int i = 0; i < width; i++)
		{
			
		}
	}
	
}