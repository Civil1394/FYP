using UnityEngine;
using System.Collections;

public class EnemyActor : TimedActor , IDamagable
{
	public int Health { get; private set; }
	[SerializeField] private HourGlass hourGlass;
	private AIBrain aiBrain;
	protected override void Start()
	{
		aiBrain = gameObject.GetComponent<AIBrain>();
		if (hourGlass != null)
		{
			OnTimerStart += hourGlass.CountTime;
			OnTimerComplete += aiBrain.TurnAction;
		}
		actionCooldown = (int)Random.Range(1f, 3f);
		base.Start();
	}
	protected override void Update()
	{
		base.Update();
	}
	
	#region IDamagable implementation

	public void SetHealth(int health)
	{
		Health = health;
	}
	public void TakeDamage(int damage)
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

	#endregion
}