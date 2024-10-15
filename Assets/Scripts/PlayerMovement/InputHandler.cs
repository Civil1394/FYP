using System;
using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour 
{
	private void Update()
	{
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
}