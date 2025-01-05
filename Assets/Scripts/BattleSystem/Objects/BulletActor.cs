using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class BulletActor : TimedActor
{
    public int Damage { get; private set; }
    public float Speed { get; private set; }
    public Vector3Int StandingPos;
    public HexDirection TargetDirection { get; set; }
    public float LifeTime { get; private set; }
    public float InitialLifeTime { get; private set; }
    public bool IsAlive { get; private set; }

    public Vector3 height_Offset { get; private set; }
    
    private List<ProjectileBehavior> behaviors = new List<ProjectileBehavior>();
    private Quaternion targetRotation;
    
    public void Initialize(int damage, float speed, Vector3Int standingPos , HexDirection direction, float lifeTime,Vector3 height_offset)
    {
        this.gameObject.tag = "Projectile";
        actionCooldown = speed;
        Damage = damage;
        Speed = speed;
        LifeTime = lifeTime;
        InitialLifeTime = lifeTime;
        IsAlive = true;
        TargetDirection = direction;
        StandingPos = standingPos;
        height_Offset = height_offset;
        
        HexCellComponent standingCell = BattleManager.Instance.hexgrid.GetCellInCoord(StandingPos);
        HexCellComponent nextCellToMove = BattleManager.Instance.hexgrid.GetCellByDirection(standingCell, direction);
        if (nextCellToMove != null)
        {
            Vector3 faceDirection = nextCellToMove.transform.position;
            // Set initial rotation
            targetRotation = Quaternion.LookRotation(faceDirection) * Quaternion.Euler(0, -90, 0);
            transform.rotation = targetRotation;
            AddBehavior<BaseBehavior>();
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
        var newBehavior = gameObject.AddComponent<T>();
        newBehavior.Initialize(this, StandingPos, TargetDirection, Speed,height_Offset);
        behaviors.Add(newBehavior);
    }


    //Activate the behaviors for the projectile
    private void Launch() 
    {
        if (!IsAlive) return;
        // Update all behaviors
        foreach (var behavior in behaviors)
        {
            behavior.UpdateBehavior(StandingPos);
        }
        LifeTime -= 1;
        CheckLifeTime();

    }

    private void CheckLifeTime()
    {
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