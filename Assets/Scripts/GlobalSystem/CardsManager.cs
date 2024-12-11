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

    private int selectedCardIndex = 0; // Track which card is currently selected
    private int max_hand_size = 2;
    public IReadOnlyList<Card> Deck => deck.AsReadOnly();
    public IReadOnlyList<Card> Hand => hand.AsReadOnly();
    public IReadOnlyList<Card> DiscardPile => discardPile.AsReadOnly();

    private System.Random random  = new System.Random();

    public Action<Card> OnCardDrawn;
    public Action<Card> OnCardPlayed;
    public Action<Card> OnCardDiscarded;
    public Action<Card> OnCardSelected; // New event for card selection
    private void Awake()
    {
        

    }

    private void Start()
    {
        max_hand_size = BattleManager.Instance.handCardsSize;
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

        // if (!BattleManager.Instance.TurnManager.CanExecuteAction(TurnActionType.PlayCard))
        // {
        //     if(BattleManager.Instance.TurnManager == null)Debug.Log("Turn Manager is null");
        //     return (Hand, DiscardPile, null);
        // }
            
        
        cardToPlay.Cast();
        hand.Remove(cardToPlay);
        OnCardPlayed.Invoke(cardToPlay);
        discardPile.Add(cardToPlay);
        
        ChainManager.Instance.RecordSuit(cardToPlay.Suit);
       //BattleManager.Instance.TurnManager.ExecuteAction(TurnActionType.PlayCard,$"Played Card {cardToPlay.Name}");
        return (Hand, DiscardPile, cardToPlay);
        
    }
    public (IReadOnlyList<Card>, IReadOnlyList<Card>, Card) PlaySelectedCard()
    {
        Card selectedCard = GetSelectedCard();
        if (selectedCard == null)
            return (Hand, DiscardPile, null);

        return PlayCard(selectedCard);
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
    
    // Method to select next card
    public void SelectFirstCard()
    {
        selectedCardIndex = 0;
        OnCardSelected?.Invoke(hand[selectedCardIndex]);
    }

    // Method to select previous card
    public void SelectSecondCard()
    {
        selectedCardIndex = 1;
        OnCardSelected?.Invoke(hand[selectedCardIndex]);
    }
    // Get currently selected card
    public Card GetSelectedCard()
    {
        if (hand.Count <= selectedCardIndex)
            return null;
        return hand[selectedCardIndex];
    }
}