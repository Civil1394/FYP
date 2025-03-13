using UnityEngine;
using System.Collections;

public class DamageDealer : MonoBehaviour
{
	protected float _damage;
	public float Damage => _damage;

	public virtual void Init(float damage)
	{
		_damage = damage;
	}
}