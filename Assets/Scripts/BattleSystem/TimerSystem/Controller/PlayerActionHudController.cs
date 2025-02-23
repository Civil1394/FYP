using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerActionHudController : Singleton<PlayerActionHudController>
{
    public bool IsAutoTrigger = false;
    public static int SectorCount =>GameConstants.AbilitySlotCount;
    
    [Header("AbilityModel Related")]
    [SerializeField] private List<AbilityOnHudModel> abilityModels = new List<AbilityOnHudModel>();
    
    private List<AbilityData> equippedAbilities;
    private PlayerActor playerActor;
    private ActionLogicHandler actionLogicHandler;
    private void Start()
    {
        
    }

    public void Initialize(List<AbilityData> equippedAbilities,PlayerActor playerActor,ActionLogicHandler actionLogicHandler)
    {
        this.equippedAbilities = equippedAbilities;
        this.playerActor = playerActor;
        this.actionLogicHandler = actionLogicHandler;
        

        int index = 0;
        foreach (var am in abilityModels)
        {
            am.Init(equippedAbilities[index].PrerequisiteChargeSteps,equippedAbilities[index].Icon);
            index++;
        }
        playerActor.OnPlayerMoved += UpdateStep;
    }
    
    public void ChangeFaceDirection(int newDirection)
    {
        if (playerActor.TryChangeFacingDirection((HexDirection)newDirection))
        {
            
        }
    }
    private void UpdateStep(HexDirection movedDirection)
    {
        abilityModels[(int)movedDirection].NotifyUpdate(1);
    }
    
}