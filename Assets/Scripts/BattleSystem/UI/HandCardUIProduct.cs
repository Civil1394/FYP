using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
public class HandCardUIProduct : MonoBehaviour
{
	[SerializeField] private TMP_Text cost;
	[SerializeField] private TMP_Text title;
	[SerializeField] private TMP_Text desc;
	[SerializeField] private Image highlight;
	[SerializeField] private Image suit;
	[Header("Suit Ref")] 
	[SerializeField] private Sprite time;
	[SerializeField] private Sprite eclipse;
	[SerializeField] private Sprite spirit;

	[SerializeField] private Vector3 preferedPosition;
	[SerializeField] private bool isSelected = false;
	private Card cardData;
	
	public void Init(Card _cardData)
	{
		this.cardData = _cardData;
		//cost.text = abilityData.InitCost.ToString();
		title.text = cardData.AbilityData.Title;
		desc.text = cardData.AbilityData.Desc;
		switch (cardData.Suit)
		{
			case Suit.Time:
				suit.sprite = time;
				break;
			case Suit.Eclipse:
				suit.sprite = eclipse;
				break;
			case Suit.Spirit:
				suit.sprite = spirit;
				break;
		}
		StartCoroutine(StorePreferredPosition());
	}
	private IEnumerator StorePreferredPosition()
	{
		// Wait for end of frame to ensure layout group has updated
		yield return new WaitForEndOfFrame();
		// Store the position after layout group has positioned the element
		preferedPosition = this.GetComponent<RectTransform>().position;
	}
	public void OnSelect()
	{
		this.DOKill();
		if (!isSelected)
		{
			isSelected = true;
			this.transform.DOMoveY(preferedPosition.y + 30f, 0.5f).SetEase(Ease.InOutQuint);
			
		}
	}

	public void OnDeselect()
	{
		this.DOKill();
		if (this.isSelected)
		{
			this.transform.DOMoveY(preferedPosition.y, 0.5f).SetEase(Ease.InOutQuint);
			isSelected = false;
		}
			
	}
	
}