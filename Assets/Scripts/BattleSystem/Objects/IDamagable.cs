using UnityEngine;
using System.Collections;
using TMPro;
public interface IDamagable
{
	float Health { get; }
	void TakeDamage(float damage);
	void HandleStatusEffectDamage(float damage);
}
