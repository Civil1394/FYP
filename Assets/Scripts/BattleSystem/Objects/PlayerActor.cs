using System;
using System.Collections;
using System.Threading;
using DG.Tweening;
using RainbowArt.CleanFlatUI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerActor : TimedActor, IDamagable
{
	public GameObject hitVFX;
	public GameObject parryVFX;
	[Header("Player Status")] private float currentHealth = 100f;

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

	public bool isTweening = false;
	private ProgressBarPattern HealthBar;

	[SerializeField] TMP_Text HealthText;

	[Header("Facing Direction")] public HexCellComponent standingCell;
	public HexDirection FacingHexDirection;
	private HexCellComponent facingCell;

	public IHexPatternHelper attackPattern;

	[Header("Hourglass trigger config")] private PlayerAction pendingAction;
	private PlayerMovement playerMovement;
	private ActionLogicHandler actionLogicHandler;
	private PendingActionVisualizer pendingActionVisualizer;



	#region events

	public event Action<HexDirection> OnPlayerMoved;
	public event Action<HexDirection> OnPlayerCast;

	#endregion

	[Header("Sound")]
	[SerializeField]private AudioClip parryClip;
	[SerializeField] private AudioClip hitClip;
	[SerializeField] private AudioClip moveClip;
	
	private Coroutine parryCoroutine;
	private GameObject parryObject;

	#region Mono

	protected void Awake()
	{
		playerMovement = GetComponent<PlayerMovement>();
		actionLogicHandler = GetComponent<ActionLogicHandler>();
		pendingActionVisualizer = GetComponent<PendingActionVisualizer>();
	}

	protected void Start()
	{
		base.Start();
	}
	public void Init(Hourglass hourglass,HexCellComponent initStandingCell,ProgressBarPattern initHealthBarPattern)
	{
		base.Init(hourglass);
		this.standingCell = initStandingCell;
		
		//Please init equipped abilities list first in the count of slots
		EquippedAbilityManager.InitEquippedAbilities(GameConstants.AbilitySlotCount);
		PlayerActionHudController.Instance.Initialize(
			EquippedAbilityManager.EquippedAbilities,
			this,
			actionLogicHandler,
			_ => { }
		);

		if (TryChangeFacingDirection(FacingHexDirection))
		{ 
			playerMovement.ChangeFacingDirection(facingCell);
		}
		
		if (hourglass != null)
		{
			OnTimerComplete += ExecutePendingAction;
			
		}
		
	}

		private void OnDestroy()
		{
			if (hourglass != null)
			{
				OnTimerStart -= _ => QueueMoveAction();    

				OnTimerComplete -= ExecutePendingAction;
			}
			
		}

	#endregion
	

	#region QueueAction
	public void QueueMoveAction()
	{
		if (BattleManager.Instance.InputHandler.inputState == InputState.CastingAbility) return;
		if (!actionLogicHandler.FacingIsLegit(FacingHexDirection)) return;
		// Replace current pending action
		pendingAction = new PlayerAction(PlayerActionType.Move, facingCell);
        
		// Visual feedback that action is queued
		ShowPendingActionPreview();
	}

	public void DequeueMoveAction()
	{
		pendingAction = null;
		pendingActionVisualizer.RemovePendingActionPointer();
	}
	private void ShowPendingActionPreview()
	{
		pendingActionVisualizer.ShowPendingActionPointer(pendingAction.Type , pendingAction.TargetCell );
	}
	
	#endregion
	
	#region ExecuteAction
	private void ExecutePendingAction()
	{
		if (pendingAction == null)
		{
			CheckTimerStatus(); // Start new timer if no action is pending
			return;
		}

		switch (pendingAction.Type)
		{
			case PlayerActionType.Move:
				ExecuteMoveAction();
				break;
			case PlayerActionType.Cast:

				break;
		}
		DequeueMoveAction();
		CheckTimerStatus();
	}
	
	private void ExecuteMoveAction()
	{
		if (pendingAction?.TargetCell == null) return;

		// Execute the actual movement
		playerMovement.Move(pendingAction.TargetCell,BattleManager.Instance.CheckCellContainChest);
        SoundManager.Instance.PlaySFX(moveClip);
		// Update cell states
		UpdateCellsStates();
		OnPlayerMoved?.Invoke(FacingHexDirection);
		FacingHexDirection = HexDirection.NONE;
		//onplayermoved
		
	}

	public void ExecuteDash(HexCellComponent targetCell, Action onFinish)
	{
		print("player dash");
		// Execute the actual movement
		playerMovement.Dash(targetCell, () =>
		{
			BattleManager.Instance.CheckCellContainChest();
			onFinish.Invoke();
		});
		BattleManager.Instance.OnPlayerMove(this, standingCell, targetCell);
		standingCell = targetCell;
		TryChangeFacingDirection(FacingHexDirection);

	}
	private void UpdateCellsStates()
	{
		BattleManager.Instance.OnPlayerMove(this, standingCell, pendingAction.TargetCell);
		standingCell = pendingAction.TargetCell;
		TryChangeFacingDirection(FacingHexDirection);
	}

	public void ExecuteCastAction(HexDirection abiltyDirection, HexCellComponent castCell)
	{
		var a = EquippedAbilityManager.GetEquippedAbilityData((int)abiltyDirection);

		actionLogicHandler.ExecuteAbility(a,castCell);
		EquippedAbilityManager.RemoveAndReplaceAbilityInDirection(abiltyDirection);
		
		//refill the empty slot in the equippedAbilities List
		while (EquippedAbilityManager.CheckAnyEmptySlotInEquippedAbilities())
		{
			PlayerActionHudController.Instance.TryAddAbility(EquippedAbilityManager.CreateAbilityInstance());
		}
	}
	
	#endregion

	public bool TryChangeFacingDirection(HexDirection tryDirection)
	{
		if (tryDirection == HexDirection.NONE)
		{
			DequeueMoveAction();
		}
		var cc = actionLogicHandler.FacingIsLegit((HexDirection)tryDirection);
		if (cc == null) return false;
		
		playerMovement.ChangeFacingDirection(cc);
		FacingHexDirection = tryDirection;
		facingCell = cc;
		QueueMoveAction();
		return true;
	}
	protected override void OverDrive()
	{
		base.OverDrive();
		Destroy(this.gameObject);
	}
	
	protected override void Collapse()
	{
		base.Collapse();
		Destroy(this.gameObject);
	}

#region IDamagable implementation

	public void InitIDamagable(float MaxHealth)
	{
		maxHealth = MaxHealth;
		currentHealth = maxHealth;
		HealthText.text = currentHealth.ToString();
		if (HealthBar == null) HealthBar = BattleManager.Instance.PlayerHealthBar;
			else Debug.LogError("No Player HealthBar found");
		HealthBar.UpdateGUIByHealthMultiplier(CalHealthBarGUIMultiplier());
	}
	private void Shake()
	{
		if (isTweening)return;
		isTweening = true;
		transform.DOShakePosition(0.2f, 1f,30).OnComplete(() => isTweening = false);
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
			
			if (damageActor != null && damageActor.CasterType != CasterType.Player)
			{
				HexDirection tempDir =
					BattleManager.Instance.hexgrid.GetFuzzyHexDirectionByCellAndPosition(standingCell,
						other.transform.position);
				if (PlayerActionHudController.Instance.CheckParryCharge(damageActor.abilityData.ColorType, tempDir))
				{
					SoundManager.Instance.PlaySFX(parryClip);
					if(parryCoroutine !=null)StopCoroutine(parryCoroutine);
					Destroy(parryObject);
					parryCoroutine = StartCoroutine(ParryVFXCoroutine());
					Destroy(other.gameObject);
					return;
				}
				damageActor.DoDamage(TakeDamage, this.gameObject, other.gameObject);
				Shake();
			}
		}
	}
	public float CalHealthBarGUIMultiplier()
	{
		float mult = maxHealth / currentHealth;
		return mult;
	}
	public void TakeDamage(float damage)
	{
		if (!BattleManager.Instance.IsPlayerInvincible)
		{
			currentHealth -= damage;
			HealthText.text = currentHealth.ToString();
			HealthBar.UpdateGUIByHealthMultiplier(CalHealthBarGUIMultiplier());
		}
		
		SoundManager.Instance.PlaySFX(hitClip);
		CameraEffectManager.Instance.PlayHitReaction();
		
		DeathCheck();
	}

	private IEnumerator ParryVFXCoroutine()
	{
		parryObject =Instantiate(parryVFX, transform.position, Quaternion.identity);
		yield return new WaitForSeconds(0.5f);
		Destroy(parryObject);
		parryCoroutine = null;
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
		Thread.Sleep(1000);
		BattleManager.Instance.LoseGame();
	}
#endregion



}

