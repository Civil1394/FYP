using UnityEngine;
using System.Collections;

public class PoisonInstance : StatusEffectInstance
{
	public override float ProcessDamageEffect(IDamagable damagable)
	{
		float damageAmount = Data.baseValue * Data.scalingFactor * CurrentStacks;
		damagable.HandleStatusEffectDamage(damageAmount);
		return damageAmount;
	}
}

