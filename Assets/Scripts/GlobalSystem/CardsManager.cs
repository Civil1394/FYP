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

    private int selectedCardIndex; // Track which card is currently selected
    private int displayedCardIndex;// Track which card is currently displayed
    private int max_hand_size = 2;
    public IReadOnlyList<Card> Deck => deck.AsReadOnly();
    public IReadOnlyList<Card> Hand => hand.AsReadOnly();
    public IReadOnlyList<Card> DiscardPile => discardPile.AsReadOnly();

    private System.Random random  = new System.Random();

    public event Action<Card> OnCardDrawn;
    public event Action<Card> OnCardPlayed;
    public event Action<Card> OnCardDiscarded;
    public event Action<Card> OnCardSelected; 
    public event Action<Card> OnCardUnSelected;
    private void Awake()
    {
        

    }

    private void Start()
    {
        max_hand_size = BattleManager.Instance.handCardsSize;
        displayedCardIndex = 99;
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
    
    // hard Code Warning
    public bool SelectCard(int cardIndex)
    {
        if (displayedCardIndex == cardIndex)
        {
            OnCardUnSelected?.Invoke(hand[cardIndex]);
            displayedCardIndex = 99;
            return false;
        }
        
        
        OnCardSelected?.Invoke(hand[cardIndex]);
        selectedCardIndex = cardIndex;
        displayedCardIndex = selectedCardIndex;
        return true;
    }
    
    // Get currently selected card
    public Card GetSelectedCard()
    {
        if (hand.Count <= selectedCardIndex)
            return null;
        return hand[selectedCardIndex];
    }

    public void ResetSelectedCard()
    {
        selectedCardIndex = 99;
        displayedCardIndex = 99;
    }
}