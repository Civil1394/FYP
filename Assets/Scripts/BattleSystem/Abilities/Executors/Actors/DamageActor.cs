using System;
using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public abstract class DamageActor : MonoBehaviour
{
	#region Events

	public abstract event Action<GameObject> OnHitApplyStatusEffect;

	#endregion
	public AbilityData abilityData;

	protected CasterType casterType;

	public CasterType CasterType
	{
		get => casterType;
		set
		{
			if(casterType == value)
			{
				return;
			}
			casterType = value;
		}
		
	}

	protected float _damage;
	public float Damage => _damage;

	public virtual void Init(float damage)
	{
		_damage = damage;
		this.tag = "DamageActor";
	}
	public virtual Type GetActualType()
	{
		return null;
	}
	public abstract void DoDamage(Action<float> damageAction, GameObject damagedTarget,GameObject source = null);
}