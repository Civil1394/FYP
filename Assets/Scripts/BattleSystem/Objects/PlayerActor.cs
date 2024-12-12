using System;
using UnityEngine;
using System.Collections;

public class PlayerActor : TimedActor , IDamagable
{
	public int Health { get; private set; }
	public HexDirection FacingHexDirection;
	public bool CanExecuteAction { get; private set; }
    
	[SerializeField] private UIHourGlass uiHourGlass;
	private PlayerAction pendingAction;
	private PlayerMovement playerMovement;
	private CastingHandler castingHandler;
	private PendingActionVisualizer pendingActionVisualizer;


	#region Mono

	protected override void Start()
	{
		uiHourGlass = BattleManager.Instance.playerUIHourGlass;
		playerMovement = GetComponent<PlayerMovement>();
		castingHandler = GetComponent<CastingHandler>();
		pendingActionVisualizer = GetComponent<PendingActionVisualizer>();
		if (uiHourGlass != null)
		{
			OnTimerStart += uiHourGlass.CountTime;
			OnTimerComplete += ExecutePendingAction;
			OnTimerComplete += DrawCardsIfEmptyHand;
		}
		base.Start();
		
		BattleManager.Instance.InputHandler.OnMoveClick.AddListener<HexCellComponent>(QueueMoveAction);
		
		//waitfor cast visual preview
		//CardsManager.Instance.OnCardSelected += 
		BattleManager.Instance.InputHandler.OnCastClick.AddListener<HexCellComponent>(QueueCastAction);
		
	}

	protected override void Update()
	{
		base.Update();
	}

	#endregion
	
	
	
	#region QueueAction
	private void QueueMoveAction(HexCellComponent targetCell)
	{
		if (!IsValidMoveTarget(targetCell)) return;
		
		// Replace current pending action
		pendingAction = new PlayerAction(PlayerActionType.Move, targetCell);
        
		// Visual feedback that action is queued
		ShowPendingActionPreview();
	}
	
	private void QueueCastAction(HexCellComponent targetCell)
	{
		Card cardToBeCast = CardsManager.Instance.GetSelectedCard();
		if(!castingHandler.CastIsLegit(cardToBeCast.AbilityData,targetCell)) return;
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
		return targetCell.CellData.CellGuiType == CellGuiType.ValidMoveRange;
	}
	//TODO: Implement better condition checker
	private bool IsValidCastTarget(HexCellComponent targetCell)
	{
		return targetCell.CellData.CellGuiType == CellGuiType.ValidMoveRange;
	}
	#endregion
	
	#region ExecuteAction
	private void ExecutePendingAction()
	{
		if (pendingAction == null)
		{
			StartNewTimer(); // Start new timer if no action is pending
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
		StartNewTimer();
	}
	
	private void ExecuteMoveAction()
	{
		if (pendingAction?.TargetCell == null) return;

		// Execute the actual movement
		playerMovement.Move(pendingAction.TargetCell);
        
		// Update cell states
		HexCellComponent playerCell = BattleManager.Instance.GetPlayerCell();
		BattleManager.Instance.OnPlayerMove(playerCell, pendingAction.TargetCell);
        
		CalNewFacingDirection(pendingAction.TargetCell);

	}

	private void ExecuteCastAction()
	{
		CardsManager.Instance.PlaySelectedCard();
		castingHandler.ExecuteAbility(pendingAction.CardToCast.AbilityData,pendingAction.TargetCell);
	}
	#endregion
	private void DrawCardsIfEmptyHand()
	{
		AbilityDatabase abilityDatabase = BattleManager.Instance.AbilityDatabase;
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
			BattleManager.Instance.hexgrid.CheckNeigborCellDirection(BattleManager.Instance.GetPlayerCell(),
				targetCell);
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