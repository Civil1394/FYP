using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;


public abstract class ProjectileBehavior : MonoBehaviour
{
    protected BulletActor BulletActor;
    protected Vector3Int standingPos;
    protected HexDirection direction;
    protected float speed;
    protected Vector3 height_offset;
    protected float lifeTime;
    public virtual void Initialize(BulletActor bulletActor, Vector3Int standingPos, HexDirection direction, float speed,
        Vector3 height_offset, float lifeTime)
    {
        this.BulletActor = bulletActor;
        this.direction = direction;
        this.speed = speed;
        this.height_offset = height_offset;
        this.lifeTime = lifeTime;
    }

    public virtual void UpdateBehavior(Vector3Int standingPos)
    {
        this.standingPos = standingPos;
    }
}

public class BaseBehavior : ProjectileBehavior
{
    
    private Tween currentMovement;  
    private  HexCellComponent nextCellToMove = new HexCellComponent();
    public override void UpdateBehavior(Vector3Int standingPos)
    {
        base.UpdateBehavior(standingPos);
        
        //TODO: Rework bullet straight behavior
        HexCellComponent standingCell = BattleManager.Instance.hexgrid.GetCellInCoord(standingPos);
        
        for (int i = 0; i < lifeTime; i++)
        {
            nextCellToMove = BattleManager.Instance.hexgrid.GetCellByDirection(standingCell, direction);
            
            //Check if reach obstacle or void then 
            if (!nextCellToMove || nextCellToMove.CellData.CellType == CellType.Invalid)
            {
                nextCellToMove = standingCell;
                currentMovement = this.transform.DOMove(standingCell.transform.position + height_offset, speed)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        BulletActor.SelfDestroy();
                    });
                return;
            }
            
            standingCell = nextCellToMove;
        }

        currentMovement = this.transform.DOMove(nextCellToMove.transform.position + height_offset, speed).SetEase(Ease.Linear);
            
        BulletActor.StandingPos = nextCellToMove.CellData.Coordinates;
    }

    private void OnDestroy()
    {
    }
}
