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
        CardsManager.Instance.OnCardDiscarded += RemoveProduct;
        CardsManager.Instance.OnCardSelected += OnCardSelected;
        CardsManager.Instance.OnCardUnSelected += OnCardDiscarded;
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

    private void OnCardSelected(Card selectedCard)
    {
        // Deselect all cards first
        foreach (var cardUI in cardIdToUIMap.Values)
        {
            cardUI.GetComponent<HandCardUIProduct>().OnDeselect();
        }

        // Select the target card if it exists
        if (selectedCard != null && cardIdToUIMap.TryGetValue(selectedCard.Id, out GameObject selectedCardUI))
        {
            selectedCardUI.GetComponent<HandCardUIProduct>().OnSelect();
        }
    }

    private void OnCardDiscarded(Card discardedCard)
    {
        if (discardedCard != null && cardIdToUIMap.TryGetValue(discardedCard.Id, out GameObject discardedCardUI))
        {
            discardedCardUI.GetComponent<HandCardUIProduct>().OnDeselect();
        }
    }
}