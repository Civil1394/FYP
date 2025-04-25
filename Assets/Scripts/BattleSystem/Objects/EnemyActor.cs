using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using RainbowArt.CleanFlatUI;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.SceneManagement;

public class EnemyActor : TimedActor, IDamagable
{
	public GameObject hitVFX;
	[SerializeField] CapsuleCollider objectCollider;
	private AIBrain aiBrain;
	public AbilityColorType abilityColor;

	public bool isTweening = false;
	[Header("IDamagable References")]
	private float currentHealth = 100f;
	public float CurrentHealth
	{
		get => currentHealth;
	}
	private float maxHealth = 100f;

	public float MaxHealth
	{
		get => maxHealth; 
		set => maxHealth = value;
	}
	
	[SerializeField] TMP_Text HealthText;
	[SerializeField] ProgressBarPattern HealthBar;
	
	[Header("Sound")]
	[SerializeField]private AudioClip parryClip;
	[SerializeField] private AudioClip hitClip;
	public override void Init(Hourglass hourglass)
	{
		base.Init(hourglass);
		
	}

	public void ExecuteDash(HexCellComponent targetCell, Action onFinish)
	{
		aiBrain.Dash(targetCell.CellData, onFinish);
	}
	protected override void Start()
	{
		aiBrain = gameObject.GetComponent<AIBrain>();
		OnTimerComplete += aiBrain.TurnAction;

		base.Start();
	}

	private void OnDestroy()
	{
		
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

	private void Shake()
	{
		if (isTweening)return;
		isTweening = true;
		transform.DOShakePosition(0.2f, 1f, 30).OnComplete(() => isTweening = false);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("DamageActor"))
		{
			
			var damageActor = other.gameObject.GetComponentInParent<DamageActor>();
			if (damageActor != null && damageActor.CasterType == CasterType.Environment)
			{
				damageActor.DoDamage(TakeDamage, this.gameObject,other.gameObject);
				Shake();
			}
			if (damageActor != null && damageActor.CasterType != CasterType.Enemy)
			{
				damageActor.DoDamage(TakeDamage, this.gameObject,other.gameObject);
				Shake();
			}
		}
	}


	public void InitIDamagable(float MaxHealth)
	{
		this.maxHealth = MaxHealth;
		currentHealth = maxHealth;
		HealthText.text = currentHealth.ToString();
		HealthBar.UpdateGUIByHealthMultiplier(CalHealthBarGUIMultiplier());
	}

	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		HealthText.text = currentHealth.ToString();
		HealthBar.UpdateGUIByHealthMultiplier(CalHealthBarGUIMultiplier());
		SoundManager.Instance.PlaySFX(hitClip);
		Instantiate(hitVFX, transform.position, Quaternion.identity);
		DeathCheck();
	}

	public void HandleStatusEffectDamage(float damage)
	{
		TakeDamage(damage);
	}

	public float CalHealthBarGUIMultiplier()
	{
		float mult = MaxHealth/CurrentHealth;
		return mult;
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

