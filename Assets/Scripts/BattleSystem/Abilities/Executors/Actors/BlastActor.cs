using System;
using UnityEngine;
using System.Collections;

public class BlastActor : DamageActor
{
	
	public GameObject BlastVFXObject { get; private set; }

	private BlastBehavior behavior;
	private BlastParameter parameter;
	
	public override event Action<GameObject> OnHitApplyStatusEffect;

	public void InitBlast(AbilityData ad, CasterType casterType, GameObject blast_VFX_Object,
		BlastParameter BlastParameter, HexDirection castingDirection, HexCellComponent casterCell)
	{
		abilityData = ad;

		this.casterType = casterType;
		this.gameObject.name = "BlastActor";
		this.tag = "DamageActor";
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

	

	public override void DoDamage(Action<float> damageAction,GameObject damagedObject,GameObject sourceVFX)
	{
		damageAction?.Invoke(_damage);
		Destroy(sourceVFX);
		OnHitApplyStatusEffect?.Invoke(damagedObject);
	}
}