using System;
using UnityEngine;
using System.Collections;

public abstract class DamageDealer : MonoBehaviour
{
	
	protected CasterType casterType;
	public CasterType CasterType => casterType;
	protected float _damage;
	public float Damage => _damage;

	public virtual void Init(float damage)
	{
		_damage = damage;
	}
	
	public abstract void DoDamage(Action<float> damageAction, GameObject source = null);
}