using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class BulletActor : TimedActor
{
    public float Damage { get; private set; }
    public float Speed { get; private set; }
    public Vector3Int StandingPos;
    public HexDirection TargetDirection { get; set; }
    public float LifeTime { get; private set; }
    public float InitialLifeTime { get; private set; }
    public bool IsAlive { get; private set; }
    public Vector3 height_Offset { get; private set; }
    public TimeType TimeType { get; private set; }
    
    private Quaternion targetRotation;
    private ProjectileBehavior behavior;

    public void InitBullet(ProjectileParameter parameter, HexDirection castingDirection, HexCellComponent casterCell)
    {
        this.gameObject.tag = "IDamagable";
        Damage = parameter.Damage;
        Speed = parameter.TravelSpeed;
        LifeTime = parameter.LifeTime;
        InitialLifeTime = parameter.LifeTime;
        TargetDirection = castingDirection;
        StandingPos = casterCell.CellData.Coordinates;
        height_Offset = parameter.VFX_Height_Offset;
        TimeType = TimeType.Boost;
        
        HexCellComponent standingCell = BattleManager.Instance.hexgrid.GetCellInCoord(StandingPos);
        HexCellComponent nextCellToMove = BattleManager.Instance.hexgrid.GetCellByDirection(standingCell, castingDirection);
        if (nextCellToMove != null)
        {
            Vector3 targetPosition = nextCellToMove.transform.position;
            targetPosition.y = transform.position.y; // Keep current y level
            transform.LookAt(targetPosition);
            AddBehavior<LinearProjectileBehavior>();
            
            //Launch when init 
            IsAlive = true;
            StartCoroutine(Launch());
        }
        else
        {
            SelfDestroy();
        }
    }
    
    //init behaviors
    private void AddBehavior<T>() where T : ProjectileBehavior
    {
        behavior = gameObject.AddComponent<T>();
        behavior.Init(this, StandingPos, TargetDirection, Speed,height_Offset, LifeTime);
    }


    //Activate the behaviors for the projectile
    private IEnumerator Launch() 
    {

      behavior.UpdateBehavior();
      yield return new WaitForSeconds(Speed);
      
    }
    

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
    }
}