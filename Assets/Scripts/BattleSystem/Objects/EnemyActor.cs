using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using TMPro;

public class EnemyActor : TimedActor, IDamagable
{
	public GameObject hitVFX;
	[SerializeField] CapsuleCollider objectCollider;
	[SerializeField] private HourglassGlobalCanvasAnimator hourglassAnimator;
	private AIBrain aiBrain;
	private float currentHealth = 100f;
	public float Health
	{
		get { return currentHealth; }
	}
	[SerializeField] TMP_Text HealthText;

	public override void Init(Hourglass hourglass)
	{
		base.Init(hourglass);

		if (hourglass == null)
		{
			OnTimerStart += hourglassAnimator.CountTime;
		}
		HealthText.text = currentHealth.ToString();
	}

	protected override void Start()
	{
		aiBrain = gameObject.GetComponent<AIBrain>();
		OnTimerComplete += aiBrain.TurnAction;

		base.Start();
	}

	private void OnDestroy()
	{
		OnTimerStart -= hourglassAnimator.CountTime;
	}

	protected override void Update()
	{
		base.Update();
	}

	

	protected override void OverDrive()
	{
		base.OverDrive();
		aiBrain.currentCell.ClearCell();
		Destroy(this.gameObject);
	}

	protected override void Collapse()
	{
		base.Collapse();
		aiBrain.currentCell.ClearCell();
		Destroy(this.gameObject);
	}

	
#region IDamagable implementation

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("DamageActor"))
		{
			var damageActor = other.gameObject.GetComponentInParent<DamageActor>();
			if (damageActor != null && damageActor.CasterType != CasterType.Enemy)
			{
				damageActor.DoDamage(TakeDamage, this.gameObject,other.gameObject);
			}

		}
	}
	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		HealthText.text = currentHealth.ToString();
		Instantiate(hitVFX, transform.position, Quaternion.identity);
		DeathCheck();
	}

	public void HandleStatusEffectDamage(float damage)
	{
		TakeDamage(damage);
	}

	protected override void DeathCheck()
	{
		if(currentHealth <= 0) OnDeath();
	}

	protected override void OnDeath()
	{
		Destroy(gameObject);
	}
#endregion
}

