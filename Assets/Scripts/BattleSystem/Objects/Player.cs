using System;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour , IDamagable
{
	public int Health  { get; private set;}
	public HexDirection FacingHexDirection;
	private void Start()
	{
		BattleManager.Instance.OnPlayerMove += CalNewFacingDirection;
	}

	public void CalNewFacingDirection(HexCellComponent targetCell)
	{
		FacingHexDirection =
			BattleManager.Instance.hexgrid.CheckNeigborCellDirection(BattleManager.Instance.GetPlayerCell(),
				targetCell);
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