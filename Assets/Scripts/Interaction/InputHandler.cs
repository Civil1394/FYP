using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
	private GameObject lastPointedObject;
	public GenericAction OnMoveClick = new GenericAction();
	public Action<HexCellComponent> OnCastClick;
	public InputState inputState = InputState.Move;
	private void Start()
	{
	}
	
	private void Update()
	{
		GetPointerEnterExist();
		GetPointerDown();
		//DrawCardController();
		RedrawCardsController();
		PlayCardController();
		
	}
	
	private void PlayCardController()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			if (CardsManager.Instance.Hand[0] != null)
				CardsManager.Instance.PlayCard(CardsManager.Instance.Hand[0]);
		}
		
		if (Input.GetKeyDown(KeyCode.E))
		{
			if(CardsManager.Instance.Hand[1]!=null)
				CardsManager.Instance.PlayCard(CardsManager.Instance.Hand[1]);
		}
	}

	private void DrawCardController()
	{
		AbilityDatabase abilityDatabase = BattleManager.Instance.AbilityDatabase;
		if (Input.GetKeyDown(KeyCode.R))
		{
			if (!BattleManager.Instance.TurnManager.CanExecuteAction(TurnActionType.DrawCard))
				return;
			if (CardsManager.Instance.Hand.Count < 3)
			{
				Card testCard = CardFactory.Instance.CreateCardFromList(abilityDatabase,"1", 
					abilityDatabase.GetRandomAbilityFromList("1").id);
				CardsManager.Instance.AddCardToDeck(testCard);
			}
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

	private void RedrawCardsController()
	{
		AbilityDatabase abilityDatabase = BattleManager.Instance.AbilityDatabase;
		if (Input.GetKeyDown(KeyCode.R))
		{
			if (!BattleManager.Instance.TurnManager.CanExecuteAction(TurnActionType.DrawCard)) 
				return;

			CardsManager.Instance.RedrawCards();
			
			for (int i = 0; i < 2; i++)
			{
				Card testCard = CardFactory.Instance.CreateCardFromList(abilityDatabase, "1",
					abilityDatabase.GetRandomAbilityFromList("1").id);
				CardsManager.Instance.AddCardToDeck(testCard);
				var (newDeck, newHand, drawnCard) = CardsManager.Instance.DrawCard();
			}
			
			BattleManager.Instance.TurnManager.ExecuteAction(TurnActionType.DrawCard, $"Drew card: redraw");
		}
	}
	
	private GameObject GetMousePointedGameObject()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		int layerMask = LayerMask.GetMask("Cell"); 
		
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			if (hit.collider.gameObject.CompareTag("Cell"))
			{
				if (hit.collider.gameObject.GetComponent<HexCellComponent>().CellData.CellType != CellType.Invalid)
				{
					return hit.collider.gameObject;
				}
			}
		}
		return null;
	}
	
	void GetPointerEnterExist()
	{
		GameObject pointedObject = GetMousePointedGameObject();

		if (pointedObject != lastPointedObject)
		{
			if (lastPointedObject != null)
			{
				// Mouse stopped pointing at lastPointedObject
				//Debug.Log("Stopped pointing at: " + lastPointedObject.name);
				//lastPointedObject.GetComponent<HexCellComponent>().ChangeCellColor(false);
			}

			if (pointedObject != null)
			{
				// Mouse started pointing at pointedObject
				//Debug.Log("Started pointing at: " + pointedObject.name);
				// Add logic for when mouse enters an object
				//pointedObject.GetComponent<HexCellComponent>().ChangeCellColor(true);
			}

			lastPointedObject = pointedObject;
		}
	}

	void GetPointerDown()
	{
		GameObject pointedObject = GetMousePointedGameObject();
		if (Input.GetKeyDown(KeyCode.Mouse0) && pointedObject)
		{
			if(inputState == InputState.Move)
				OnMoveClick.Invoke(pointedObject.GetComponent<HexCellComponent>());
			if (inputState == InputState.CastingAbility)
			{
				OnCastClick?.Invoke(pointedObject.GetComponent<HexCellComponent>());
				inputState = InputState.Move;
			}
				
		}
	}
	
	public void SetInputState(InputState newState)
	{
		inputState = newState;
	}
	
}
public enum InputState
{
	Move,
	CastingAbility
}