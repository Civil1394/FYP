using System;
using UnityEngine;
using System.Collections;

public class HandCardContainer : MonoBehaviour
{
	[SerializeField] private GameObject handCardPrefab;
	private void Start()
	{
		CardsManager.Instance.OnCardDrawn += GenerateNewProduct;

	}

	private void GenerateNewProduct(Card card)
	{
		handCardPrefab.GetComponent<HandCardUIProduct>().Init(card.AbilityData);
	}
}