using System;
using UnityEngine;

public class Card : ICard
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Desc { get; private set; }
    public Suit Suit { get; private set; }
    public AbilityData AbilityData { get; private set; }

    public Card()
    {
        Id = Helpers.GetUniqueID(this);
    }
    

    public void SetSuit(Suit suit) => Suit = suit;
    
    public void SetAbilityData(AbilityData abilityData)
    {
        AbilityData = abilityData;
        Name = abilityData.Title;
    }

    public void Cast()
    {
        //BattleManager.Instance.ActionLogicHandler.HandleCast(this.AbilityData);

    }
    
}