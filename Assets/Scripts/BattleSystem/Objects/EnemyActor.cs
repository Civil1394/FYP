using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class EnemyActor : TimedActor , IDamagable
{
	public float Health { get; private set; }
	public AbilityData abilityData;
	
	[SerializeField] CapsuleCollider objectCollider;
	[SerializeField] private GlobalCanvasHourGlass GlobalCanvasHourGlass;
	private AIBrain aiBrain;
	
	
	protected override void Start()
	{
		aiBrain = gameObject.GetComponent<AIBrain>();
		SetHealth(aiBrain.enemyConfig.Health);
		if (GlobalCanvasHourGlass != null)
		{
			OnTimerStart += GlobalCanvasHourGlass.CountTime;
			OnTimerComplete += aiBrain.TurnAction;
		}
		actionCooldown = (int)Random.Range(1f, 5f);
		base.Start();
	}
	protected override void Update()
	{
		base.Update();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Projectile"))
		{
			Debug.Log("gethit");
			//TakeDamage(other.GetComponent<BulletActor>().Damage);
			var bullet = other.GetComponent<BulletActor>();
			TimeManipulate(bullet.timeManipulationType,bullet.Speed);
			aiBrain.currentCell.SetType(CellType.Empty);
			Destroy(other.gameObject);
		}
	}

	#region IDamagable implementation

	public void SetHealth(float health)
	{
		Health = health;
	}
	public void TakeDamage(float damage)
	{
		Health -= damage;
		if (Health <= 0)
		{
			Debug.Log("Player is dead");
			Destroy(this.gameObject);
		}
		
	}

	public void Heal(float heal)
	{
		Health += heal;
	}

	#endregion
}