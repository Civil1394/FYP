using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlayerActor : TimedActor,IDamagable
{
	[Header("Player Status")]
	private float currentHealth = 100f;
	public float Health
	{
		get { return currentHealth; }
	}

	[SerializeField] TMP_Text HealthText;

	[Header("Facing Direction")]
	public HexCellComponent standingCell;
	public HexDirection FacingHexDirection;
	private HexCellComponent facingCell;
	
	public bool CanExecuteAction { get; private set; }
	public IHexPatternHelper attackPattern;
	
	[Header("Hourglass trigger config")]
	private PlayerAction pendingAction;
	private PlayerMovement playerMovement;
	private ActionLogicHandler actionLogicHandler;
	private PendingActionVisualizer pendingActionVisualizer;

	private HourglassOnHudAnimator hourglassOnHudAnimator;
	#region events
	public event Action<HexDirection> OnPlayerMoved;
	public event Action<HexDirection> OnPlayerCast;
	#endregion
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
	public void Init(Hourglass hourglass,HexCellComponent initStandingCell, HourglassOnHudAnimator hourglassAnimator )
	{
		base.Init(hourglass);
		this.standingCell = initStandingCell;
		this.hourglassOnHudAnimator = hourglassAnimator;
		
		//Please init equipped abilities list first in the count of slots
		EquippedAbilityManager.InitEquippedAbilities(GameConstants.AbilitySlotCount);
		PlayerActionHudController.Instance.Initialize(EquippedAbilityManager.EquippedAbilities,
												this,
															actionLogicHandler,
												direction => ExecuteCastAction(direction));

		if (TryChangeFacingDirection(FacingHexDirection))
		{ 
			playerMovement.ChangeFacingDirection(facingCell);
		}
		
		if (hourglass != null)
		{
			//OnTimerStart += _ => QueueMoveAction();
			OnTimerComplete += ExecutePendingAction;
		}

		if (hourglassOnHudAnimator != null)
		{
			OnTimerStart += hourglassOnHudAnimator.CountTime;
		}
		
		HealthText.text = currentHealth.ToString();
		
	}

	private void OnDestroy()
	{
		if (hourglass != null)
		{
			OnTimerStart -= _ => QueueMoveAction();    
			OnTimerStart -= hourglassOnHudAnimator.CountTime;
			OnTimerComplete -= ExecutePendingAction;
		}
		
	}

	#endregion
	

	#region QueueAction
	public void QueueMoveAction()
	{
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
		playerMovement.Move(pendingAction.TargetCell);
        
		// Update cell states
		UpdateCellsStates();
		OnPlayerMoved?.Invoke(FacingHexDirection);
		FacingHexDirection = HexDirection.NONE;
		//onplayermoved
		
	}

	private void UpdateCellsStates()
	{
		// foreach (var c in attackPattern.GetPattern(playerCell.CellData))
		// {
		// 	c.SetGuiType(CellGuiType.Empty);
		// }
		BattleManager.Instance.OnPlayerMove(this, standingCell, pendingAction.TargetCell);
		standingCell = pendingAction.TargetCell;
		TryChangeFacingDirection(FacingHexDirection);
		// foreach (var c in attackPattern.GetPattern(pendingAction.TargetCell.CellData))
		// {
		// 	c.SetGuiType(CellGuiType.ValidAttackCell);
		// }
	}

	private void ExecuteCastAction(HexDirection direction)
	{
		var a = EquippedAbilityManager.GetEquippedAbilityData((int)direction);

		actionLogicHandler.ExecuteAbility(a,direction);
		EquippedAbilityManager.RemoveAbilityInDirection(direction);
		
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

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("DamageActor"))
		{
			var damageActor = other.gameObject.GetComponentInParent<DamageActor>();
			if (damageActor != null && damageActor.CasterType != CasterType.Player)
			{
				damageActor.DoDamage(TakeDamage, other.gameObject);
			}

		}
	}
	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		HealthText.text = currentHealth.ToString();
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

