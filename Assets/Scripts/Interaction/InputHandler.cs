using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class InputHandler : MonoBehaviour
{
	private GameObject lastPointedObject;
	private bool isRotating = false;
	public GenericAction OnMoveClick = new GenericAction();
	public GenericAction OnCastClick = new GenericAction();
	public InputState inputState = InputState.Move;
	private HexDirection selectedAbility = HexDirection.NONE;

	public HexDirection SelectedAbility
	{
		get { return selectedAbility; }
		set { selectedAbility = value; }
	}	
	[SerializeField] private CinemachineVirtualCamera playerCamera;
	private CinemachineOrbitalTransposer orbitalTransposer;

	private int cameraRotationCnt;
	private void Start()
	{
		orbitalTransposer = playerCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
		orbitalTransposer.m_XAxis.Value = -45;
		selectedAbility = HexDirection.NONE;
	}
	
	private void Update()
	{
		ScrollToRotateOrbitalCamera();
		ResetOrbitalCameraAngle();
		UpdateCameraRotationCnt();
		GetPointerEnterExist();
		GetPointerDown();
        OnShiftDown();
        OnRightClick();
        OnSixDirectionKeyPress();
		OnAltSixDirectionKeyPress();
	}


	#region Dumb code

		// private void SelectPlayer()
    	// {
    	// 	if (Input.GetKeyDown(KeyCode.Alpha1))
    	// 	{
    	// 		BattleManager.Instance.MultipleCharacterControlSystem.SwitchCharacter(0);
    	// 		return;
    	// 	}
    	//
    	// 	if (Input.GetKeyDown(KeyCode.Alpha2))
    	// 	{
    	// 		BattleManager.Instance.MultipleCharacterControlSystem.SwitchCharacter(1);
    	// 		return;
    	// 	}
    	//
    	// 	if (Input.GetKeyDown(KeyCode.Alpha3))
    	// 	{
    	// 		BattleManager.Instance.MultipleCharacterControlSystem.SwitchCharacter(2);
    	// 		return;
    	// 	}
    	//
    	// 	if (Input.GetKeyDown(KeyCode.Alpha4))
    	// 	{
    	// 		BattleManager.Instance.MultipleCharacterControlSystem.SwitchCharacter(3);
    	// 		return;
    	// 	}
    	// }
    	// private void RedrawCardsController()
    	// {
    	// 	AbilityDatabase abilityDatabase = BattleManager.Instance.AbilityDatabase;
    	// 	if (Input.GetKeyDown(KeyCode.R))
    	// 	{
    	// 		if (!BattleManager.Instance.TurnManager.CanExecuteAction(TurnActionType.DrawCard)) 
    	// 			return;
    	//
    	// 		CardsManager.Instance.RedrawCards();
    	// 		
    	// 		for (int i = 0; i < 2; i++)
    	// 		{
    	// 			Card testCard = CardFactory.Instance.CreateCardFromList(abilityDatabase, "1",
    	// 				abilityDatabase.GetRandomAbilityFromList("1").id);
    	// 			CardsManager.Instance.AddCardToDeck(testCard);
    	// 			var (newDeck, newHand, drawnCard) = CardsManager.Instance.DrawCard();
    	// 		}
    	// 		
    	// 		BattleManager.Instance.TurnManager.ExecuteAction(TurnActionType.DrawCard, $"Drew card: redraw");
    	// 	}
    	// }

	#endregion

	
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
			print("pointer down");
			if (selectedAbility == HexDirection.NONE) return;
			print(selectedAbility.ToString());
			if (!pointedObject) return;
			PlayerActionHudController.Instance.CastAbility(selectedAbility, pointedObject.GetComponent<HexCellComponent>());
			//if(inputState == InputState.Move)
			//	OnMoveClick.Invoke(pointedObject.GetComponent<HexCellComponent>());
			//if (inputState == InputState.CastingAbility)
			//{
			//	OnCastClick?.Invoke(pointedObject.GetComponent<HexCellComponent>());
			//}
		}
	}

	void OnRightClick()
	{
		if (Input.GetMouseButtonDown(1))
		{
			selectedAbility = HexDirection.NONE;
		}
	}
	void ResetOrbitalCameraAngle()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			orbitalTransposer.m_XAxis.Value = -45;
		}
	}
	void OnRightMosueButtonDrag()
	{
		PlayerActionHudController.Instance.UpdateRotation(orbitalTransposer.m_XAxis.Value);
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

	void ScrollToRotateOrbitalCamera()
	{
		PlayerActionHudController.Instance.UpdateRotation(orbitalTransposer.m_XAxis.Value);
		//Controller of allowing the orbitalTransposer rotate head movement towards player object by dragging the mouse in x axis
		if(isRotating)return;
		if (Input.GetAxisRaw("Mouse ScrollWheel")>0&& orbitalTransposer != null)
		{
			isRotating = true;
			DOTween.To(
				() => orbitalTransposer.m_XAxis.Value,
				x => orbitalTransposer.m_XAxis.Value = x,
				orbitalTransposer.m_XAxis.Value + 60,
				0.1f).OnComplete(
				() =>
				{
					isRotating = false;
				});
		}
		else if(Input.GetAxisRaw("Mouse ScrollWheel")<0&& orbitalTransposer != null)
		{
			isRotating = true;
			DOTween.To(
				() => orbitalTransposer.m_XAxis.Value,
				x => orbitalTransposer.m_XAxis.Value = x,
				orbitalTransposer.m_XAxis.Value - 60,
				0.1f).OnComplete(
				() =>
				{
					isRotating = false;
				});
		}
	}

	void OnShiftDown()
	{
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			inputState = InputState.CastingAbility;
		}
		if (Input.GetKeyUp(KeyCode.LeftControl))
		{
			inputState = InputState.Move;
		}
	}

	void UpdateCameraRotationCnt()
	{
		float cameraAngle = orbitalTransposer.m_XAxis.Value;
		cameraAngle = ToPositiveAngle(cameraAngle);
		cameraRotationCnt = 6 - (int)((cameraAngle + 45) / 60);
		PlayerActionHudController.Instance.cameraRotationCnt = cameraRotationCnt;
	}
	void OnSixDirectionKeyPress()
	{
		if (inputState == InputState.CastingAbility)
		{
			return;
		}
		#region Movement input
		if (Input.GetKeyDown(KeyCode.W))
		{
			//default nw
			int tempDir = ((int)HexDirection.NW + cameraRotationCnt) % 6;
			PlayerActionHudController.Instance.ChangeFaceDirection(tempDir);
		}
		else if(Input.GetKeyDown(KeyCode.E))
		{
			//default ne
			int tempDir = ((int)HexDirection.NE + cameraRotationCnt) % 6;
			PlayerActionHudController.Instance.ChangeFaceDirection(tempDir);
		}
		else if(Input.GetKeyDown(KeyCode.D))
		{
			//default e
			int tempDir = ((int)HexDirection.E + cameraRotationCnt) % 6;
			PlayerActionHudController.Instance.ChangeFaceDirection(tempDir);
		}
		else if(Input.GetKeyDown(KeyCode.X))
		{
			//default se
			int tempDir = ((int)HexDirection.SE + cameraRotationCnt) % 6;
			PlayerActionHudController.Instance.ChangeFaceDirection(tempDir);
		}
		else if(Input.GetKeyDown(KeyCode.Z))
		{
			//default sw
			int tempDir = ((int)HexDirection.SW + cameraRotationCnt) % 6;
			PlayerActionHudController.Instance.ChangeFaceDirection(tempDir);
		}
		else if(Input.GetKeyDown(KeyCode.A))
		{
			//default w
			int tempDir = ((int)HexDirection.W + cameraRotationCnt) % 6;
			PlayerActionHudController.Instance.ChangeFaceDirection(tempDir);
		}
		else if(Input.GetKeyDown(KeyCode.S))
		{
			PlayerActionHudController.Instance.ChangeFaceDirection((int)HexDirection.NONE);
		}
		#endregion
	}

	void OnAltSixDirectionKeyPress()
	{
		if (inputState == InputState.Move)
		{
			return;
		}
		#region Ability input
		if (Input.GetKeyDown(KeyCode.W))
		{
			//default nw
			int tempDir = ((int)HexDirection.NW + cameraRotationCnt) % 6;
			selectedAbility = PlayerActionHudController.Instance.SelectAbility(tempDir);
		}
		else if(Input.GetKeyDown(KeyCode.E))
		{
			//default ne
			int tempDir = ((int)HexDirection.NE + cameraRotationCnt) % 6;
			selectedAbility = PlayerActionHudController.Instance.SelectAbility(tempDir);
        }
        else if(Input.GetKeyDown(KeyCode.D))
		{
			//default e
			int tempDir = ((int)HexDirection.E + cameraRotationCnt) % 6;
            selectedAbility = PlayerActionHudController.Instance.SelectAbility(tempDir);
        }
        else if(Input.GetKeyDown(KeyCode.X))
		{
			//default se
			int tempDir = ((int)HexDirection.SE + cameraRotationCnt) % 6;
			selectedAbility = PlayerActionHudController.Instance.SelectAbility(tempDir);
        }
        else if(Input.GetKeyDown(KeyCode.Z))
		{
			//default sw
			int tempDir = ((int)HexDirection.SW + cameraRotationCnt) % 6;
			selectedAbility = PlayerActionHudController.Instance.SelectAbility(tempDir);
        }
        else if(Input.GetKeyDown(KeyCode.A))
		{
			//default w
			int tempDir = ((int)HexDirection.W + cameraRotationCnt) % 6;
			selectedAbility = PlayerActionHudController.Instance.SelectAbility(tempDir);
        }
        #endregion
    }
	public void SetInputState(InputState newState)
	{
		inputState = newState;
	}
	float ToPositiveAngle(float angle)
	{
		angle = angle % 360;
		while(angle < 0) {
			angle += 360;
		}
		return angle;
	}
}
public enum InputState
{
	Move,
	CastingAbility
}