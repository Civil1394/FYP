using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "ProjectileParameters", menuName = "Effect/ProjectileParameters")]
public class ProjectileParameters : EffectParameters
{
	public int Damage;
	public float FlowSpeed;
	public float LifeTime;
	// Direction will be determined at runtime
}