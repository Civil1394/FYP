using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using Vector3 = System.Numerics.Vector3;

public abstract class ProjectileBehavior : MonoBehaviour
{
    protected BulletActor BulletActor;
    protected Vector3Int standingPos;
    protected HexDirection direction;
    protected float speed;
    
    public virtual void Initialize(BulletActor bulletActor, Vector3Int standingPos,HexDirection direction, float speed)
    {
        this.BulletActor = bulletActor;
        this.direction = direction;
        this.speed = speed;
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
    private bool pendingDestroy = false;
    public override void Initialize(BulletActor bulletActor, Vector3Int standingPos, HexDirection direction, float speed)
    {
        base.Initialize(bulletActor, standingPos, direction, speed);
    }
    public override void UpdateBehavior(Vector3Int standingPos)
    {
        base.UpdateBehavior(standingPos);
        
        HexCellComponent standingCell = BattleManager.Instance.hexgrid.GetCellInCoord(standingPos);
        
        for (int i = 0; i < speed; i++)
        {
            nextCellToMove = BattleManager.Instance.hexgrid.GetCellByDirection(standingCell, direction);
            
            if (!nextCellToMove || nextCellToMove.CellData.CellType == CellType.Invalid)
            {
                nextCellToMove = standingCell;
                pendingDestroy = true;
                currentMovement = this.transform.DOMove(standingCell.transform.position, BattleManager.Instance.InitTurnDur)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        BulletActor.SelfDestroy();
                    });
                return;
            }
            
            standingCell = nextCellToMove;
        }

        currentMovement = this.transform.DOMove(nextCellToMove.transform.position, BattleManager.Instance.InitTurnDur).SetEase(Ease.Linear);
            
        BulletActor.StandingPos = nextCellToMove.CellData.Coordinates;
    }

    private void OnTurnEnd()
    {
        //speedup the movement if the movement tween is still playing
        if (currentMovement != null && currentMovement.IsPlaying())
        {
            currentMovement.Kill();
            currentMovement = this.transform.DOMove(nextCellToMove.transform.position, 0.2f).SetEase(Ease.Linear)
                .OnComplete(
                    () =>
                    {
                        if(pendingDestroy)
                            BulletActor.SelfDestroy();
                        return;
                    });
        }
       
    }

    private void OnDestroy()
    {
        BattleManager.Instance.TurnManager.OnTurnEnd -= OnTurnEnd;
    }
}
