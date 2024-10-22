using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandCardUIProduct : MonoBehaviour,IPointerClickHandler
{
	[SerializeField] private TMP_Text cost;
	[SerializeField] private TMP_Text title;
	[SerializeField] private TMP_Text desc;
	[SerializeField] private Image suit;
	[Header("Suit Ref")] 
	[SerializeField] private Sprite time;
	[SerializeField] private Sprite eclipse;
	[SerializeField] private Sprite spirit;

	private Card cardData;
	public void Init(Card _cardData)
	{
		this.cardData = _cardData;
		//cost.text = abilityData.initCost.ToString();
		title.text = cardData.AbilityData.title;
		desc.text = cardData.AbilityData.desc;
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
	}


	public void OnPointerClick(PointerEventData eventData)
	{
		CardsManager.Instance.PlayCard(this.cardData);
	}
}