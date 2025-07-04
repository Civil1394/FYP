﻿using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class ProjectileActor : DamageActor
{
    public float TravelSpeed { get; private set; }
    public Vector3Int StandingPos;
    public HexDirection TargetDirection { get; set; }
    public float LifeTime { get; private set; }
    public Vector3 height_Offset { get; private set; }
    private ProjectileParameter parameter;
    private ProjectileBehavior behavior;
    public override event Action<GameObject> OnHitApplyStatusEffect;
    
    private List<HexCellComponent> highlightedCells = new List<HexCellComponent>();
    public void InitBullet(AbilityData ad, CasterType casterType, ProjectileParameter parameter, HexDirection castingDirection,
        HexCellComponent castDirectionCell, Transform casterObjectTransform)
    {
        abilityData = ad;
        this.casterType = casterType;
        this.gameObject.tag = "DamageActor";
        this.parameter = parameter;
        
        
        _damage = parameter.Damage;
        TravelSpeed = parameter.TravelSpeed;
        LifeTime = parameter.LifeTime;
        
        TargetDirection = castingDirection;
        StandingPos = castDirectionCell.CellData.Coordinates;
        height_Offset = parameter.VFX_Height_Offset;

        HexCellComponent standingCell = BattleManager.Instance.hexgrid.GetCellInCoord(StandingPos);
        HexCellComponent nextCellToMove = BattleManager.Instance.hexgrid.GetCellByDirection(standingCell, castingDirection);
        if (nextCellToMove != null)
        {
            Vector3 targetPosition = nextCellToMove.transform.position;
            targetPosition.y = transform.position.y; // Keep current y level
            this.transform.localRotation = Quaternion.Euler(0,HexDirectionHelper.DeltaDegreeRotationForProjectile(castingDirection),0);
            AddBehavior(parameter.BehaviorType);
            
            //Launch when init 
            StartCoroutine(Launch());
        }
        else
        {
            Destroy(gameObject);
        }
    }


    //init behaviors
    private void AddBehavior(ProjectileBehavior.BehaviorType behaviorType) 
    {
        switch (behaviorType)
        {
            case ProjectileBehavior.BehaviorType.Linear:
                LinearProjectileBehavior linearProjectileBehavior = gameObject.AddComponent<LinearProjectileBehavior>();
                behavior = linearProjectileBehavior;
                behavior.Init(this, StandingPos, TargetDirection, TravelSpeed,height_Offset, LifeTime);
                break;
            case ProjectileBehavior.BehaviorType.Parabola:
                ParabolaProjectileBehavior parabolaProjectileBehavior = gameObject.AddComponent<ParabolaProjectileBehavior>();
                behavior = parabolaProjectileBehavior;
                behavior.Init(this, StandingPos, TargetDirection, TravelSpeed,height_Offset, LifeTime);
                parabolaProjectileBehavior.InitParabolaBlast(parameter.ParabolaBlastVFX,
                    parameter.BlastDamage,
                    casterType,
                    (target) => 
                        abilityData.ApplyStatusEffects(AbilityStatusApplicationType.OverTime, target));
                break;
            default:
                Debug.LogErrorFormat("{0} is not a valid behavior type.", behaviorType);
                break;
        }
    }


    //Activate the behaviors for the projectile
    private IEnumerator Launch() 
    {

      behavior.UpdateBehavior();
      yield return new WaitForSeconds(TravelSpeed);
      
    }
    

    public override void DoDamage(Action<float> damageAction,GameObject damagedObject, GameObject source = null)
    {
        damageAction?.Invoke(_damage);
        if (parameter.IsSelfDestructOnCollision)
        {
            Destroy(this.gameObject);
        }
        OnHitApplyStatusEffect?.Invoke(damagedObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cell"))
        {
            var currentCell = other.GetComponent<HexCellComponent>();
            print("hit cell");
            currentCell.HighLightCell(abilityData.ColorType);
            highlightedCells.Add(currentCell);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cell"))
        {
            other.GetComponent<HexCellComponent>().UnhighLightCell();
        }
    }

    private void OnDestroy()
    {
        foreach (var c in highlightedCells)
        {
            c.UnhighLightCell();
        }
    }
}