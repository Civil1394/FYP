using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
public class PlayerActionHudController : Singleton<PlayerActionHudController>
{
    public bool IsAutoTrigger = false;
    public static int SectorCount =>GameConstants.AbilitySlotCount;
    [SerializeField] RectTransform objectTransform;
    [Header("AbilityModel Related")]
    [SerializeField] private List<AbilityOnHudModel> abilityModels = new List<AbilityOnHudModel>();
    
    private List<AbilityData> equippedAbilities;
    private PlayerActor playerActor;
    private ActionLogicHandler actionLogicHandler;
    private void Start()
    {
        
    }

    public void Initialize(List<AbilityData> equippedAbilities,PlayerActor playerActor,ActionLogicHandler actionLogicHandler,Action<HexDirection> ExecuteCastCallback)
    {
        this.equippedAbilities = equippedAbilities;
        this.playerActor = playerActor;
        this.actionLogicHandler = actionLogicHandler;
        

        int index = 0;
        foreach (var am in abilityModels)
        {
            am.Init((HexDirection)index, equippedAbilities[index].PrerequisiteChargeSteps, equippedAbilities[index].Icon,equippedAbilities[index].ColorType, ExecuteCastCallback);
            index++;
        }
        playerActor.OnPlayerMoved += UpdateDirectionStep;
        playerActor.OnPlayerCast += ResetDirectionStep;
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
    
    private void ResetDirectionStep(HexDirection triggeredDirection)
    {
        
        abilityModels[(int)triggeredDirection].Reset();
    }

    //rotate according to orbital transposer
    public void UpdateRotation(float cameraRotation)
    {
        float delta = 45;
        if(objectTransform == null) Debug.LogWarning("Action Hud Object transform is null");
        objectTransform.DOLocalRotate(new Vector3(0,0,cameraRotation + delta), .2f);
    }
}