using UnityEngine;
using System.Collections;

public interface IProjectile 
{
	public int Damage { get; }
	public float Speed { get;}
	public Vector3 Direction { get; }
	public float LifeTime { get;}
	public bool IsAlive { get;}
	public void Initialize(int damage, float speed, Vector3 direction, float lifeTime);
}