using System;
using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour 
{
	private GameObject lastPointedObject;
	private void Update()
	{
		GetPointerEnterExist();
		DrawCardController();
		PlayCardController();
	}
	
	private void PlayCardController()
	{
		if (CardsManager.Instance.Hand.Count == 0) return;
			
		if (Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKeyDown(KeyCode.Mouse1))
			CardsManager.Instance.PlayCard(CardsManager.Instance.Hand[0]);

		if (Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKeyDown(KeyCode.Mouse1) )
			if(CardsManager.Instance.Hand.Count >1)
				CardsManager.Instance.PlayCard(CardsManager.Instance.Hand[1]);

		if (Input.GetKeyDown(KeyCode.Mouse1) && !Input.GetKeyDown(KeyCode.Mouse0))
			if(CardsManager.Instance.Hand.Count >2)
				CardsManager.Instance.PlayCard(CardsManager.Instance.Hand[2]);
		
	}

	private void DrawCardController()
	{
		AbilityDatabase abilityDatabase = BattleManager.Instance.AbilityDatabase;
		if (Input.GetKeyDown(KeyCode.R))
		{
			if (CardsManager.Instance.Hand.Count < 3)
			{
				Card testCard = CardFactory.Instance.CreateCardFromList(abilityDatabase,"1", abilityDatabase.GetRandomAbilityFromList("1").id);
				CardsManager.Instance.AddCardToDeck(testCard);
			}
			var (newDeck, newHand, drawnCard) = CardsManager.Instance.DrawCard();
			if (drawnCard != null)
			{
				Debug.Log($"Drew card: {drawnCard.Name}");
			}
			else
			{
				Debug.Log("No cards left in the deck");
			}

		}
	}
	private GameObject GetMousePointedGameObject()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit))
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
				lastPointedObject.GetComponent<HexCellComponent>().ChangeCellColor(false);
			}

			if (pointedObject != null)
			{
				// Mouse started pointing at pointedObject
				//Debug.Log("Started pointing at: " + pointedObject.name);
				// Add logic for when mouse enters an object
				pointedObject.GetComponent<HexCellComponent>().ChangeCellColor(true);
			}

			lastPointedObject = pointedObject;
		}
	}
}