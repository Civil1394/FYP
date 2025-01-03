using System;
using UnityEngine;

public class Card : ICard
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Desc { get; private set; }
    public int Cost {get; private set; }
    public Suit Suit { get; private set; }
    public AbilityData AbilityData { get; private set; }

    public Card()
    {
        GenerateUniqueId();
    }

    private void GenerateUniqueId()
    {
        Id = $"CARD_{Guid.NewGuid().ToString("N")}";
    }

    public void SetSuit(Suit suit) => Suit = suit;
    
    public void SetAbilityData(AbilityData abilityData)
    {
        AbilityData = abilityData;
        Name = abilityData.Title;
        Cost = abilityData.InitCost;
    }

    public void Cast()
    {
        //BattleManager.Instance.CastingHandler.HandleCast(this.AbilityData);

    }
    
}