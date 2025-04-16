using UnityEngine;
using System.Collections;
using RainbowArt.CleanFlatUI;
using TMPro;
public interface IDamagable
{
	float Health { get; }
	void TakeDamage(float damage);
	void HandleStatusEffectDamage(float damage);
}
