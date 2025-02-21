using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerActionHudController : Singleton<PlayerActionHudController>
{
    public bool IsAutoTrigger = false;
    public static int SectorCount = 6;
    [SerializeField] private List<Button> directionButton = new List<Button>();
    private List<AbilityData> embeddedAbilityDatas;
    private PlayerActor playerActor;
    private ActionLogicHandler actionLogicHandler;
    private HexGrid hexGrid;
    private void Start()
    {
        hexGrid = BattleManager.Instance.hexgrid;
        
    }

    public void Initialize(List<AbilityData> embeddedAbilityDatas,PlayerActor playerActor,ActionLogicHandler actionLogicHandler)
    {
        this.embeddedAbilityDatas = embeddedAbilityDatas;
        this.playerActor = playerActor;
        this.actionLogicHandler = actionLogicHandler;
        
        playerActor.OnPlayerMoved += UpdateStep;
    }
    
    public void ChangeFaceDirection(int newDirection)
    {
        var cc = actionLogicHandler.FacingIsLegit((HexDirection)newDirection);
        if (cc == null) return;
    
        playerActor.OnPlayerFacingDirectionLegalChanged?.Invoke(cc);
    }
    private void UpdateStep()
    {
        Debug.Log("UpdateStep");
    }
    
    private void OnThresholdReached(int value)
    {
        HexCellComponent playerCell = BattleManager.Instance.PlayerCell;
        HexDirection facingDirection = playerActor.FacingHexDirection;
        HexCellComponent targetCell = hexGrid.GetCellByDirection(playerCell, facingDirection);
        if(actionLogicHandler.CastIsLegit(embeddedAbilityDatas[value],targetCell) == false) return;
        
        embeddedAbilityDatas[value].TriggerAbility(playerActor.transform,
                                                    facingDirection,
                                                    playerCell,HourglassInventory.Instance.hourglassesList[0].TimeType);
    }
}