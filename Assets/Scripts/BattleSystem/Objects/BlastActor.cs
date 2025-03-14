using UnityEngine;
using System.Collections;

public class BlastActor : DamageDealer
{
	public GameObject BlastVFXObject { get; private set; }
	
	private BlastBehavior behavior;
	private BlastParameter parameter;
	public void InitBlast(GameObject blast_VFX_Object,BlastParameter BlastParameter , HexDirection castingDirection, HexCellComponent casterCell)
	{
		this.gameObject.name = "BlastActor";
		this.parameter = BlastParameter;
		base.Init(parameter.Damage);
		this.BlastVFXObject = blast_VFX_Object;
		AddBehavior<LinearBlastBehavior>(castingDirection,casterCell);
		StartCoroutine(Launch());

	}

	private void AddBehavior<T>(HexDirection direction, HexCellComponent casterCell) where T : BlastBehavior
	{
		behavior = gameObject.AddComponent<T>();
		behavior.Init(BlastVFXObject,parameter.Width,direction,casterCell);
	}


	private IEnumerator Launch()
	{
		int stepCount = parameter.BlastStepCount;
		while (stepCount > 0)
		{
			behavior.UpdateBehavior();
			yield return new WaitForSeconds(parameter.BlastStepDelay);
			stepCount--;
		}
		Destroy(gameObject);
	}
	
}