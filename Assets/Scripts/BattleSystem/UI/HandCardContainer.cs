using System;
using System.Collections.Generic;
using UnityEngine;

public class HandCardContainer : MonoBehaviour
{
    [SerializeField] private GameObject handCardPrefab;
    private Dictionary<string, GameObject> cardIdToUIMap = new Dictionary<string, GameObject>();

    private void Start()
    {
        CardsManager.Instance.OnCardDrawn += GenerateNewProduct;
        CardsManager.Instance.OnCardPlayed += RemoveProduct;
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDestroy()
    {
        CardsManager.Instance.OnCardDrawn -= GenerateNewProduct;
        CardsManager.Instance.OnCardPlayed -= RemoveProduct;
    }

    private void GenerateNewProduct(Card card)
    {
        GameObject _handCardPrefab = Instantiate(handCardPrefab, this.transform);
        _handCardPrefab.GetComponent<HandCardUIProduct>().Init(card);
        cardIdToUIMap[card.Id] = _handCardPrefab;
    }

    private void RemoveProduct(Card card)
    {
        if (cardIdToUIMap.TryGetValue(card.Id, out GameObject cardUIObject))
        {
            Destroy(cardUIObject);
            cardIdToUIMap.Remove(card.Id);
        }
        else
        {
            Debug.LogWarning($"Tried to remove card UI for card {card.Id}, but it was not found in the container.");
        }
    }
}