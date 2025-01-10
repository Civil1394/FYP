using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Cinemachine;
public class InputHandler : MonoBehaviour
{
	private GameObject lastPointedObject;
	public GenericAction OnMoveClick = new GenericAction();
	public GenericAction OnCastClick = new GenericAction();
	public InputState inputState = InputState.Move;
	
	[SerializeField] private CinemachineVirtualCamera playerCamera;
	private CinemachineOrbitalTransposer orbitalTransposer;
	private void Start()
	{
		orbitalTransposer = playerCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
	}
	
	private void Update()
	{
		OnRightMosueButtonDrag();
		
		GetPointerEnterExist();
		GetPointerDown();
		RedrawCardsController();
		SelectCardController();
		
	}
	
	private void SelectCardController()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			CardsManager.Instance.SelectFirstCard();
			SetInputState(InputState.CastingAbility);
		}
		else if (Input.GetKeyDown(KeyCode.E))
		{
			CardsManager.Instance.SelectSecondCard();
			SetInputState(InputState.CastingAbility);
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

	void OnRightMosueButtonDrag()
	{
		//Controller of allowing the orbitalTransposer rotate head movement towards player object by dragging the mouse in x axis
		if (Input.GetMouseButtonDown(1) && orbitalTransposer != null)  
		{
			orbitalTransposer.m_XAxis.m_InputAxisName = "Mouse X";
		}
		else if (Input.GetMouseButtonUp(1) && orbitalTransposer != null)
		{
			orbitalTransposer.m_XAxis.m_InputAxisName = "";
			orbitalTransposer.m_XAxis.m_InputAxisValue = 0;
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