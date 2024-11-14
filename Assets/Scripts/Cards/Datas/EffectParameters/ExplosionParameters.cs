using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "ExplosionParameters", menuName = "Effect/ExplosionParameters")]
public class ExplosionParameters : EffectParameters
{
	public float radius;
	public float damage;
	public float force;
}