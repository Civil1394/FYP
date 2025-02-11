using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHourGlassController : MonoBehaviour
{
    protected  List<AbilityData> passiveAbilityDatas { get;private set; }
    protected  HashSet<float> pendingThresholds { get; private set; }
    private CastingHandler castingHandler;
    private HexGrid hexGrid;
    
    private HashSet<float> triggeredThresholdFlags = new HashSet<float>();
    private PlayerActor playerActor;
    private void Start()
    {
        playerActor = GetComponent<PlayerActor>();
        castingHandler = GetComponent<CastingHandler>();
        hexGrid = BattleManager.Instance.hexgrid;
        
    }

    public void Initialize(HashSet<float> triggerThreshold, List<AbilityData> passiveAbilityDatas)
    {
        this.pendingThresholds = triggerThreshold;
        this.passiveAbilityDatas = passiveAbilityDatas;
    }

    public void ThresholdCheck(float remainingTimePercent)
    {
        foreach (float threshold in pendingThresholds)
        {
            if (!triggeredThresholdFlags.Contains(threshold) && remainingTimePercent <= threshold)
            {
                triggeredThresholdFlags.Add(threshold);
                OnThresholdReached(triggeredThresholdFlags.Count -1);
                Debug.Log($"Threshold {threshold * 100}% reached!");
            }
            
        }
    }

    public void ClearTriggeredThresholdFlags()
    {
        triggeredThresholdFlags.Clear();
    }
    public void OnThresholdReached(int value)
    {
        HexCellComponent playerCell = BattleManager.Instance.PlayerCell;
        HexDirection facingDirection = playerActor.FacingHexDirection;
        HexCellComponent targetCell = hexGrid.GetCellByDirection(playerCell, facingDirection);
        if(castingHandler.CastIsLegit(passiveAbilityDatas[value],targetCell) == false) return;
        
        passiveAbilityDatas[value].TriggerAbility(playerActor.transform,
                                                    facingDirection,
                                                    playerCell);
    }
}