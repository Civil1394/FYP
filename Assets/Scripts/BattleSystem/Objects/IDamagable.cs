using UnityEngine;
using System.Collections;
using RainbowArt.CleanFlatUI;
using TMPro;
public interface IDamagable
{
	float CurrentHealth { get; }
	float MaxHealth { get; }
	void InitIDamagable(float MaxHealth);
	void TakeDamage(float damage);
	void HandleStatusEffectDamage(float damage);
	float CalHealthBarGUIMultiplier();
}
