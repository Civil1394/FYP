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
	
	private HourglassUIAnimator hourglassUIAnimator;
	private PlayerAction pendingAction;
	private PlayerMovement playerMovement;
	private ActionLogicHandler actionLogicHandler;
	private PendingActionVisualizer pendingActionVisualizer;

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

	public void Init(Hourglass hourglass,HexCellComponent initStandingCell)
	{
		base.Init(hourglass);
		this.standingCell = initStandingCell;
		
		PlayerActionHudController.Instance.Initialize(AbilityManager.EquippedAbilities,
												this,
															actionLogicHandler,
												(direction => ExecuteCastAction(direction)));

		if (TryChangeFacingDirection(FacingHexDirection))
		{ 
			playerMovement.ChangeFacingDirection(facingCell);
		}
		
		if (hourglass != null)
		{
			OnTimerStart += _ => QueueMoveAction();    
			OnTimerComplete += ExecutePendingAction;
			//OnTimerComplete += DrawCardsIfEmptyHand;
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
			OnTimerComplete -= ExecutePendingAction;
			//OnTimerComplete -= DrawCardsIfEmptyHand;
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
	
	public void QueueCastAction(HexCellComponent targetCell)
	{
		Card cardToBeCast = CardsManager.Instance.GetSelectedCard();
		if(actionLogicHandler.CastIsLegit(cardToBeCast.AbilityData,targetCell) == false) return;
		// Replace current pending action
		pendingAction = new PlayerAction(PlayerActionType.Cast, targetCell,cardToBeCast);
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
		var a = AbilityManager.GetEquippedAbility((int)direction);

		actionLogicHandler.ExecuteAbility(a,direction);
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
	// private void DrawCardsIfEmptyHand()
	// {
	// 	
	// 	if (CardsManager.Instance.Hand.Count == 0)
	// 	{
	// 		for (int i = 0; i < BattleManager.Instance.handCardsSize; i++)
	// 		{
	// 			Card testCard = CardFactory.Instance.CreateCardFromList(abilityDatabase,"1", 
	// 				abilityDatabase.GetRandomAbilityFromList("1").id);
	// 			CardsManager.Instance.AddCardToDeck(testCard);
	// 			var (newDeck, newHand, drawnCard) = CardsManager.Instance.DrawCard();
	// 			if (drawnCard != null)
	// 			{
	// 				BattleManager.Instance.TurnManager.ExecuteAction(TurnActionType.DrawCard, $"Drew card: {drawnCard.Name}");
	// 				Debug.Log($"Drew card: {drawnCard.Name}");
	// 			}
	// 			else
	// 			{
	// 				Debug.Log("No cards left in the deck");
	// 			}
	// 		}
	// 		
	// 	}
	// }
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