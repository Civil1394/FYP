using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChainUIProduct : MonoBehaviour
{
	[Header("Suit Ref")] 
	[SerializeField] private Sprite time;
	[SerializeField] private Sprite eclipse;
	[SerializeField] private Sprite spirit;
	
	private Image suitImage;
	
	public void Init(Suit _suit)
	{
		suitImage = this.GetComponent<Image>();
		switch (_suit)
		{
			case Suit.Time:
				suitImage.sprite = time;
				break;
			case Suit.Eclipse:
				suitImage.sprite = eclipse;
				break;
			case Suit.Spirit:
				suitImage.sprite = spirit;
				break;
		}
	}
}