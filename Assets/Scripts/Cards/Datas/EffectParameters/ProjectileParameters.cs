using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "ProjectileParameters", menuName = "Effect/ProjectileParameters")]
public class ProjectileParameters : EffectParameters
{
	public int damage;
	public float speed;
	public float lifeTime;
	// Direction will be determined at runtime
}