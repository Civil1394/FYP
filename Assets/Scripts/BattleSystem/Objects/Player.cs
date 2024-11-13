using System;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour , IDamagable
{
	public int Health  { get; private set;}

	private void Start()
	{
		
	}

	public void SetHealth(int health)
	{
		Health = health;
	}
	public void Damage(int damage)
	{
		Health -= damage;
		if (Health <= 0)
		{
			Debug.Log("Player is dead");
		}
	}

	public void Heal(int heal)
	{
		Health += heal;
	}
}