using System;
using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public abstract class DamageActor : MonoBehaviour
{
	#region Events

	public abstract event Action<GameObject> OnHitApplyStatusEffect;

	#endregion
	
	protected CasterType casterType;
	public CasterType CasterType => casterType;
	protected float _damage;
	public float Damage => _damage;

	public virtual void Init(float damage)
	{
		_damage = damage;
	}
	
	public abstract void DoDamage(Action<float> damageAction, GameObject damagedTarget,GameObject source = null);
}