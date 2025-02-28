using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerActor : TimedActor
{
	public float Health { get; private set; }
	
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
			OnTimerStart += _ => QueueMoveAction();    
			OnTimerComplete += ExecutePendingAction;
		}

		if (hourglassOnHudAnimator != null)
		{
			OnTimerStart += hourglassOnHudAnimator.CountTime;
		}
		
	}

	protected override void Start()
	{
		//attackPattern = PresetPatterns.AoePattern(2);
		
		base.Start();
		
		
		//abandoned
		//BattleManager.Instance.InputHandler.OnMoveClick.AddListener<HexCellComponent>(QueueMoveAction);
		//BattleManager.Instance.InputHandler.OnCastClick.AddListener<HexCellComponent>(QueueCastAction);
		

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
				//ExecuteCastAction();
				break;
		}

		pendingActionVisualizer.RemovePendingActionPointer();
		pendingAction = null;
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
		var cc = actionLogicHandler.FacingIsLegit((HexDirection)tryDirection);
		if (cc == null) return false;
		
		playerMovement.ChangeFacingDirection(cc);
		FacingHexDirection = tryDirection;
		facingCell = cc;
		QueueMoveAction();
		return true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Projectile"))
		{
			Debug.Log("gethit");
			//TakeDamage(other.GetComponent<BulletActor>().Damage);
			var bullet = other.GetComponent<BulletActor>();
			TimeManipulate(bullet.TimeType,bullet.Speed);
			Debug.Log(bullet.TimeType);
			Destroy(other.gameObject);
		}
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
	
}