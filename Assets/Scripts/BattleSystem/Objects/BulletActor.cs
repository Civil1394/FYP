using System;
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
    public void Initialize(float damage, float speed, Vector3Int standingPos , HexDirection direction, float lifeTime,Vector3 height_offset,TimeType type)
    {
        this.gameObject.tag = "Projectile";
        ActionCooldown = speed;
        Damage = damage;
        Speed = speed;
        LifeTime = lifeTime;
        InitialLifeTime = lifeTime;
        IsAlive = true;
        TargetDirection = direction;
        StandingPos = standingPos;
        height_Offset = height_offset;
        TimeType = type;
        
        HexCellComponent standingCell = BattleManager.Instance.hexgrid.GetCellInCoord(StandingPos);
        HexCellComponent nextCellToMove = BattleManager.Instance.hexgrid.GetCellByDirection(standingCell, direction);
        if (nextCellToMove != null)
        {
            Vector3 faceDirection = nextCellToMove.transform.position;
            // Set initial rotation
            targetRotation = Quaternion.LookRotation(faceDirection) * Quaternion.Euler(0, -90, 0);
            transform.rotation = targetRotation;
            AddBehavior<LinearBehavior>();
            
            //Launch once when init 
            Launch();
            OnTimerComplete += Launch;
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
        behavior.Initialize(this, StandingPos, TargetDirection, Speed,height_Offset, LifeTime);
    }


    //Activate the behaviors for the projectile
    private void Launch() 
    {
        if (!IsAlive) return;
        LifeTime -= behavior.UpdateBehavior();
        
        CheckLifeTime();
    }

    private void CheckLifeTime()
    {
        //Debug.Log(LifeTime);
        if (LifeTime <= 0)
        {
            IsAlive = false;
            SelfDestroy();
        }
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
    }
}