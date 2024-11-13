using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour,IProjectile
{
	public int Damage { get; private set; }
	public float Speed { get; private set; }
	public Vector3 Direction { get; private set; }
	public float LifeTime { get; private set; }
	public bool IsAlive { get; private set; }

	public void Initialize(int damage, float speed, Vector3 direction, float lifeTime)
	{
		Damage = damage;
		Speed = speed;
		Direction = direction;
		LifeTime = lifeTime;
	}

	public void Launch()
	{
		
	}
}