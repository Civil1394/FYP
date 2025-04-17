using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// View of equippedAbilityQueue on Hud
/// </summary>
public class PlayerActionHudController : Singleton<PlayerActionHudController>
{
    public bool IsAutoTrigger = false;
    public static int SectorCount =>GameConstants.AbilitySlotCount;
    
    [SerializeField] RectTransform objectTransform;
    
    [Header("AbilityModel Related")]
    [SerializeField] private List<AbilityOnHudModel> abilityModels = new List<AbilityOnHudModel>();
    public int[] rotationZs = { 210, 270, 330, 30, 90, 150 };
    public int cameraRotationCnt;
    
    // This list acts as a queue for the abilities displayed on the HUD.
    //private List<AbilityData> equippedAbilities;
    private PlayerActor playerActor;
    private ActionLogicHandler actionLogicHandler;
    
    // This callback is passed in from outside (when initializing) to execute the cast.
    private Action<HexDirection> executeCastCallback;
    
    private bool isShowHudModels = false;
    private void Start()
    {
    }
    
    /// <summary>
    /// Initializes the HUD with a list of abilities.
    /// </summary>
    /// <param name="equippedAbilities">The ordered list of abilities.</param>
    /// <param name="playerActor">Reference to the player actor.</param>
    /// <param name="actionLogicHandler">Reference to the action logic handler.</param>
    /// <param name="ExecuteCastCallback">Callback to execute when an ability is cast.</param>
    public void Initialize(List<AbilityData> equippedAbilities,PlayerActor playerActor,ActionLogicHandler actionLogicHandler,Action<HexDirection> ExecuteCastCallback)
    {
        //this.equippedAbilities = new List<AbilityData>(equippedAbilities);
        this.playerActor = playerActor;
        this.actionLogicHandler = actionLogicHandler;
        this.executeCastCallback = ExecuteCastCallback;
        
        RefreshHUD();
        
        playerActor.OnPlayerMoved += UpdateDirectionStep;
        playerActor.OnPlayerCast += OnAbilityCast;
        isShowHudModels = true;
    }
    
    /// <summary>
    /// Refreshes all HUD models based on the current equippedAbilities queue.
    /// </summary>
    public void RefreshHUD()
    {
        Dictionary<AbilityData, (int chargedSteps, bool fullyCharged)> abilityChargeStates = 
            new Dictionary<AbilityData, (int, bool)>();
        
        for (int i = 0; i < abilityModels.Count && i < EquippedAbilityManager.EquippedAbilities.Count; i++)
        {
            AbilityData abilityData = abilityModels[i].localAbilityData;
            if (abilityData != null)
            {
                int chargedSteps = abilityModels[i].GetChargedSteps();
                bool fullyCharged = abilityModels[i].FullyCharged;
                abilityChargeStates[abilityData] = (chargedSteps, fullyCharged);
            }
        }
        
        for (int i = 0; i < abilityModels.Count; i++)
        {
            if (i < EquippedAbilityManager.EquippedAbilities.Count)
            {
                AbilityData abilityData = EquippedAbilityManager.EquippedAbilities[i];
                
                abilityModels[i].Init((HexDirection)i, abilityData, OnSelectAbility);
            
                // Check if we have saved charge state for this ability data
                if (abilityChargeStates.TryGetValue(abilityData, out var chargeState))
                {
                    if (chargeState.fullyCharged)
                    {
                        abilityModels[i].SetFullyCharged();
                    }
                    else if (chargeState.chargedSteps > 0)
                    {
                        abilityModels[i].RestoreChargeSteps(chargeState.chargedSteps);
                    }
                }
            }
            else
            {
                // If there is no ability for this slot, reset or clear it.
                abilityModels[i].Reset();
            }
        }
    }
    
    /// <summary>
    /// Called when the player casts an ability. This method:
    /// 1. Invokes the external cast callback.
    /// 2. Resets all HUD models and shifts the remaining abilities forward.
    /// </summary>
    /// <param name="castDirection">The direction (slot) of the cast ability.</param>
    private void OnAbilityCast(HexDirection castDirection)
    {
        // Execute external cast logic.
        executeCastCallback?.Invoke(castDirection);
        
    }
    
    
    /// <summary>
    /// API method that allows an external class to add a new ability into the queue,
    /// but only if there is an empty HUD model at the last position.
    /// </summary>
    /// <param name="newAbility">The new ability to add.</param>
    /// <returns>True if the ability was added, false otherwise.</returns>
    public bool TryAddAbility(AbilityData newAbility)
    {
        if (EquippedAbilityManager.EquippedAbilities.Count < abilityModels.Count)
        {
            //equippedAbilities.Add(newAbility);
            RefreshHUD();
            return true;
        }
        return false;
    }
    
    public void ChangeFaceDirection(int newDirection)
    {
        if (playerActor.TryChangeFacingDirection((HexDirection)newDirection))
        {
            
        }
    }
    private void UpdateDirectionStep(HexDirection movedDirection)
    {
        abilityModels[(int)movedDirection].NotifyUpdate(1);
    }

    public bool CheckParryCharge(AbilityColorType cType, HexDirection dir)
    {
        if(abilityModels[(int)dir] == null) return false;
        if (abilityModels[(int)dir].localAbilityData.ColorType == cType)
        {
            abilityModels[(int)dir].NotifyUpdate(1);
            return true;
        }
        return false;
    }
    public HexDirection SelectAbility(int inputDirection)
    {
        if (!abilityModels[inputDirection].FullyCharged) return HexDirection.NONE;
        abilityModels[inputDirection].ShowAttackPattern();
        SwitchToShowHudModels(false);
        BattleManager.Instance.InputHandler.SetInputState(InputState.CastingAbility);
        playerActor.DequeueMoveAction();
        return(HexDirection)inputDirection;
    }
    public void CastAbility(HexDirection abiltyDirection, HexCellComponent castCell)
    {
        if(abiltyDirection == HexDirection.NONE) return;
        if (!IsCastCellLegit(abiltyDirection, castCell)) return;
        if (!abilityModels[(int)abiltyDirection].FullyCharged) return;
        abilityModels[(int)abiltyDirection].UseAbility(abiltyDirection,castCell);
        SwitchToShowHudModels(true);
        RefreshHUD();
        
    }
    public bool IsCastCellLegit(HexDirection abiltyDirection, HexCellComponent castCell)
    {
        AbilityData tempAd = abilityModels[(int)abiltyDirection].localAbilityData;
        List<HexCell> selectableCells = tempAd.SelectablePattern
            .GetPattern(BattleManager.Instance.PlayerCell.CellData).ToList();
        if (selectableCells.Contains(castCell.CellData)) return true;
        return false;
    }
    //rotate according to orbital transposer camera
    public void UpdateRotation(float cameraRotation)
    {
        float delta = 45;
        if(objectTransform == null) Debug.LogWarning("Action Hud Object transform is null");
        objectTransform.localRotation = Quaternion.Euler(0, 0, cameraRotation+delta);
        //objectTransform.DOLocalRotate(new Vector3(0,0,cameraRotation + delta), .2f);
    }

    public void SwapAbilitySlot(HexDirection a, HexDirection b)
    {
        (abilityModels[(int)a], abilityModels[(int)b]) = (abilityModels[(int)b], abilityModels[(int)a]);
        (EquippedAbilityManager.EquippedAbilities[(int)a], EquippedAbilityManager.EquippedAbilities[(int)b]) = (EquippedAbilityManager.EquippedAbilities[(int)b], EquippedAbilityManager.EquippedAbilities[(int)a]);
        abilityModels[(int)a].UpdateDirection(a);
        abilityModels[(int)b].UpdateDirection(b);
        abilityModels[(int)a].UpdateRotation(rotationZs[(int)a]);
    }

    public void OnSelectAbility()
    {
        playerActor.DequeueMoveAction();
        SwitchToShowHudModels(false);
    }
    public void SwitchToShowHudModels(bool isShow)
    {
        //isShowHudModels = !isShowHudModels;
        foreach (var abilityModel in abilityModels)
        {
            var c = abilityModel.GetComponent<CanvasGroup>();
            if (isShow)
            {
                c.alpha = 1;
            }
            else
            {
                c.alpha = 0;
            }
        }
    }

    public void UnshowAbilityPreview(HexDirection direction)
    {
        abilityModels[(int)direction].UnshownAttackPattern();
    }
}