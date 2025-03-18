using System;
using System.Collections.Generic;
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
    }
    
    /// <summary>
    /// Refreshes all HUD models based on the current equippedAbilities queue.
    /// </summary>
    private void RefreshHUD()
    {
        for (int i = 0; i < abilityModels.Count; i++)
        {
            if (i < EquippedAbilityManager.EquippedAbilities.Count)
            {
                var abilityData = EquippedAbilityManager.EquippedAbilities[i];
                abilityModels[i].Init((HexDirection)i, abilityData.PrerequisiteChargeSteps, abilityData.Icon, abilityData.ColorType, OnAbilityCast);
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
        
        // Refresh the entire HUD so that all remaining abilities are shifted forward.
        RefreshHUD();
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
        abilityModels[(int)a].UpdateDirection(a);
        abilityModels[(int)b].UpdateDirection(b);
        abilityModels[(int)a].UpdateRotation(rotationZs[(int)a]);
    }
}