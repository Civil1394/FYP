using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerActor : TimedActor
{
	public float Health { get; private set; }
	public HexDirection FacingHexDirection;
	public bool CanExecuteAction { get; private set; }
	public IHexPatternHelper attackPattern;
	
	[Header("Hourglass trigger config")]
	
	private HourglassUIAnimator hourglassUIAnimator;
	private PlayerAction pendingAction;
	private PlayerMovement playerMovement;
	private ActionLogicHandler actionLogicHandler;
	private PendingActionVisualizer pendingActionVisualizer;
	private PlayerActionHudController actionHudController;
	private AbilityDatabase abilityDatabase;

	#region events
	public Action<HexCellComponent> OnPlayerFacingDirectionLegalChanged;
	public event Action OnPlayerMoved;

	#endregion
	#region Mono

	protected override void Start()
	{
		//attackPattern = PresetPatterns.AoePattern(2);
		playerMovement = GetComponent<PlayerMovement>();
		OnPlayerFacingDirectionLegalChanged += playerMovement.ChangeFacingDirection;
		OnPlayerFacingDirectionLegalChanged += QueueMoveAction;
		actionLogicHandler = GetComponent<ActionLogicHandler>();
		pendingActionVisualizer = GetComponent<PendingActionVisualizer>();
		actionHudController = GetComponent<PlayerActionHudController>();
		
		base.Start();
		//if(hourglassUIAnimator == null)hourglassUIAnimator = BattleManager.Instance.playerHourglassUIAnimator;
		//else Debug.LogError("HourglassUIAnimator is null");
		if (hourglass != null)
		{
			//OnTimerStart += hourglassUIAnimator.CountTime;
			OnTimerComplete += ExecutePendingAction;
			OnTimerComplete += DrawCardsIfEmptyHand;
		}
		
		//abandoned
		//BattleManager.Instance.InputHandler.OnMoveClick.AddListener<HexCellComponent>(QueueMoveAction);
		//BattleManager.Instance.InputHandler.OnCastClick.AddListener<HexCellComponent>(QueueCastAction);
		
		abilityDatabase = BattleManager.Instance.AbilityDatabase;
		PlayerActionHudController.Instance.Initialize(abilityDatabase.GetAbilityList("1"),this,actionLogicHandler);

	}

	private void OnDestroy()
	{
		if (hourglass != null)
		{
			//OnTimerStart -= hourglassUIAnimator.CountTime;
			OnTimerComplete -= ExecutePendingAction;
			OnTimerComplete -= DrawCardsIfEmptyHand;
		}
		
	}

	#endregion
	

	#region QueueAction
	public void QueueMoveAction(HexCellComponent targetCell)
	{
		if (!IsValidMoveTarget(targetCell)) return;
		
		// Replace current pending action
		pendingAction = new PlayerAction(PlayerActionType.Move, targetCell);
        
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
	
	private bool IsValidMoveTarget(HexCellComponent targetCell)
	{
		return targetCell.CellData.CellGuiType == CellGuiType.ValidMoveCell;
	}
	//TODO: Implement better condition checker
	private bool IsValidCastTarget(HexCellComponent targetCell)
	{
		return targetCell.CellData.CellGuiType == CellGuiType.ValidMoveCell;
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
				ExecuteCastAction();
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
		OnPlayerMoved?.Invoke();
		//CalNewFacingDirection(pendingAction.TargetCell);

	}

	private void UpdateCellsStates()
	{
		HexCellComponent playerCell = BattleManager.Instance.PlayerCell;
		// foreach (var c in attackPattern.GetPattern(playerCell.CellData))
		// {
		// 	c.SetGuiType(CellGuiType.Empty);
		// }
		BattleManager.Instance.OnPlayerMove(this, playerCell, pendingAction.TargetCell);
		// foreach (var c in attackPattern.GetPattern(pendingAction.TargetCell.CellData))
		// {
		// 	c.SetGuiType(CellGuiType.ValidAttackCell);
		// }
	}

	private void ExecuteCastAction()
	{
		CardsManager.Instance.PlaySelectedCard();
		actionLogicHandler.ExecuteAbility(pendingAction.CardToCast.AbilityData,pendingAction.TargetCell);
		CardsManager.Instance.ResetSelectedCard();
	}
	#endregion
	private void DrawCardsIfEmptyHand()
	{
		
		if (CardsManager.Instance.Hand.Count == 0)
		{
			for (int i = 0; i < BattleManager.Instance.handCardsSize; i++)
			{
				Card testCard = CardFactory.Instance.CreateCardFromList(abilityDatabase,"1", 
					abilityDatabase.GetRandomAbilityFromList("1").id);
				CardsManager.Instance.AddCardToDeck(testCard);
				var (newDeck, newHand, drawnCard) = CardsManager.Instance.DrawCard();
				if (drawnCard != null)
				{
					BattleManager.Instance.TurnManager.ExecuteAction(TurnActionType.DrawCard, $"Drew card: {drawnCard.Name}");
					Debug.Log($"Drew card: {drawnCard.Name}");
				}
				else
				{
					Debug.Log("No cards left in the deck");
				}
			}
			
		}
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