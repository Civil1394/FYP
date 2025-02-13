using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerAutoTriggerController))]
public class PlayerActor : TimedActor
{
	public float Health { get; private set; }
	public HexDirection FacingHexDirection;
	public bool CanExecuteAction { get; private set; }
	public IHexPatternHelper attackPattern;
	
	[Header("Hourglass trigger config")]
	[SerializeField] private List<float> thresholdList = new List<float>();
	
	private UIHourGlassView uiHourGlassView;
	private PlayerAction pendingAction;
	private PlayerMovement playerMovement;
	private CastingHandler castingHandler;
	private PendingActionVisualizer pendingActionVisualizer;
	private PlayerAutoTriggerController autoTriggerController;
	private AbilityDatabase abilityDatabase;

	#region Mono

	protected override void Start()
	{
		attackPattern = new CustomOffsetPattern(
			new Vector3Int(0, 0, -3),
			new Vector3Int(1, 0, -3),
			new Vector3Int(0, 0, -4),
			new Vector3Int(-2, 0, 1),
			new Vector3Int(-2, 0, 2),
			new Vector3Int(-3, 0, 2),
			new Vector3Int(3, 0, 1),
			new Vector3Int(2, 0, 2),
			new Vector3Int(3, 0, 2)
		);
		
		if(uiHourGlassView == null)uiHourGlassView = BattleManager.Instance.playerUIHourGlassView;
		else Debug.LogError("UIHourGlassView is null");
		if (uiHourGlassView != null)
		{
			OnTimerStart += uiHourGlassView.CountTime;
			OnTimerComplete += ExecutePendingAction;
			OnTimerComplete += DrawCardsIfEmptyHand;
		}
		
		playerMovement = GetComponent<PlayerMovement>();
		castingHandler = GetComponent<CastingHandler>();
		pendingActionVisualizer = GetComponent<PendingActionVisualizer>();
		autoTriggerController = GetComponent<PlayerAutoTriggerController>();
		
		
		base.Start();
		
		BattleManager.Instance.InputHandler.OnMoveClick.AddListener<HexCellComponent>(QueueMoveAction);
		
		//waitfor cast visual preview
		//CardsManager.Instance.OnCardSelected += 
		BattleManager.Instance.InputHandler.OnCastClick.AddListener<HexCellComponent>(QueueCastAction);
		
		abilityDatabase = BattleManager.Instance.AbilityDatabase;
		HashSet<float> pendingThresholds = new HashSet<float>(thresholdList);
		autoTriggerController.Initialize(pendingThresholds, abilityDatabase.GetAbilityList("1"));
		OnTimerTick += autoTriggerController.ThresholdCheck;
		OnTimerComplete += autoTriggerController.ClearTriggeredThresholdFlags;

	}

	private void OnDestroy()
	{
		if (uiHourGlassView != null)
		{
			OnTimerStart -= uiHourGlassView.CountTime;
			OnTimerComplete -= ExecutePendingAction;
			OnTimerComplete -= DrawCardsIfEmptyHand;
		}
		
		OnTimerTick -= autoTriggerController.ThresholdCheck;
		OnTimerComplete -= autoTriggerController.ClearTriggeredThresholdFlags;
	}

	protected override void Update()
	{
		base.Update();
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
		if(castingHandler.CastIsLegit(cardToBeCast.AbilityData,targetCell) == false) return;
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
		//CalNewFacingDirection(pendingAction.TargetCell);

	}

	private void UpdateCellsStates()
	{
		HexCellComponent playerCell = BattleManager.Instance.PlayerCell;
		foreach (var c in attackPattern.GetPattern(playerCell.CellData))
		{
			c.SetGuiType(CellGuiType.Empty);
		}
		BattleManager.Instance.OnPlayerMove(this, playerCell, pendingAction.TargetCell);
		foreach (var c in attackPattern.GetPattern(pendingAction.TargetCell.CellData))
		{
			c.SetGuiType(CellGuiType.ValidAttackCell);
		}
	}

	private void ExecuteCastAction()
	{
		CardsManager.Instance.PlaySelectedCard();
		castingHandler.ExecuteAbility(pendingAction.CardToCast.AbilityData,pendingAction.TargetCell);
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
	public void CalNewFacingDirection(HexCellComponent targetCell)
	{
		FacingHexDirection =
			BattleManager.Instance.hexgrid.CheckNeigborCellDirection(BattleManager.Instance.PlayerCell,
				targetCell);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Projectile"))
		{
			Debug.Log("gethit");
			//TakeDamage(other.GetComponent<BulletActor>().Damage);
			var bullet = other.GetComponent<BulletActor>();
			TimeManipulate(bullet.TimeType,bullet.Speed);
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