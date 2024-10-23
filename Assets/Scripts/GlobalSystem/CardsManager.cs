using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class CardsManager : Singleton<CardsManager>
{
    private List<Card> deck = new List<Card>();
    private List<Card> hand = new List<Card>();
    private List<Card> discardPile = new List<Card>();
    
    public IReadOnlyList<Card> Deck => deck.AsReadOnly();
    public IReadOnlyList<Card> Hand => hand.AsReadOnly();
    public IReadOnlyList<Card> DiscardPile => discardPile.AsReadOnly();

    private System.Random random  = new System.Random();

    public Action<Card> OnCardDrawn;
    public Action<Card> OnCardPlayed;
    public Action<Card> OnCardDiscarded;
    private TurnManager turnManager;
    private void Awake()
    {
        

    }

    private void Start()
    {
        if (turnManager == null) turnManager = BattleManager.Instance.turnManager;
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

        if (!turnManager.CanExecuteAction(TurnActionType.PlayCard))
        {
            return (Hand, DiscardPile, null);
        }
            
        
        cardToPlay.TriggerCard();
        hand.Remove(cardToPlay);
        OnCardPlayed.Invoke(cardToPlay);
        discardPile.Add(cardToPlay);
        
        ChainManager.Instance.RecordSuit(cardToPlay.Suit);
        turnManager.ExecuteAction(TurnActionType.PlayCard,
            $"Played Card {cardToPlay.Name}");
        return (Hand, DiscardPile, cardToPlay);
        
    }
    
    public (IReadOnlyList<Card>, IReadOnlyList<Card>) RedrawCards()
    {
       
        var cardsToRedraw = new List<Card>(hand);

        foreach (var cardToPlay in cardsToRedraw)
        {
            if (cardToPlay != null)
            {
                
            }
            hand.Remove(cardToPlay);
            OnCardDiscarded?.Invoke(cardToPlay); 
            discardPile.Add(cardToPlay);
        }

        return (Hand, DiscardPile);
    }
}