using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class CardsManager : Singleton<CardsManager>
{
    private List<Card> deck;
    private List<Card> hand;
    private List<Card> discardPile;
    
    public IReadOnlyList<Card> Deck => deck.AsReadOnly();
    public IReadOnlyList<Card> Hand => hand.AsReadOnly();
    public IReadOnlyList<Card> DiscardPile => discardPile.AsReadOnly();

    private System.Random random;

    public Action<Card> OnCardDrawn;
    public Action<Card> OnCardPlayed;
    private void Awake()
    {
        random = new System.Random();
        deck = new List<Card>();
        hand = new List<Card>();
        discardPile = new List<Card>();
    }
    
    public void AddCardToDeck(Card newCard)
    {
        deck.Add(newCard);
    }


    public (IReadOnlyList<Card>, IReadOnlyList<Card>, Card) DrawCard()
    {
        if (!deck.Any())
        {
            return (Deck, Hand, null);
        }

        var index = random.Next(deck.Count);
        var drawnCard = deck[index];
        var newCard = SetRandomSuit(drawnCard);

        deck.RemoveAt(index);
        hand.Add(newCard);
        OnCardDrawn?.Invoke(newCard);
            
        return (Deck, Hand, newCard);
    }

    private Card SetRandomSuit(Card card)
    {
        var randomSuit = (Suit)random.Next(System.Enum.GetValues(typeof(Suit)).Length);
        card.SetSuit(randomSuit);
        return card;
    }

    public (IReadOnlyList<Card>, IReadOnlyList<Card>, Card) PlayCard(Card cardToPlay)
    {
        if (!hand.Contains(cardToPlay))
        {
            return (Hand, DiscardPile, null);
        }

        if (CostManager.Instance.MinusAvailCost(cardToPlay.Cost))
        {
            cardToPlay.TriggerCard();
            hand.Remove(cardToPlay);
            OnCardPlayed.Invoke(cardToPlay);
            discardPile.Add(cardToPlay);
            return (Hand, DiscardPile, cardToPlay);
        }
        
        return (Hand, DiscardPile, null);
    }
    
}