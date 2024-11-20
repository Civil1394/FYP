using UnityEngine;
using System.Collections;
using DG.Tweening;
using Vector3 = System.Numerics.Vector3;

public abstract class ProjectileBehavior : MonoBehaviour
{
    protected Bullet bullet;
    protected Vector3Int standingPos;
    protected HexDirection direction;
    protected float speed;
    
    public virtual void Initialize(Bullet bullet, Vector3Int standingPos,HexDirection direction, float speed)
    {
        this.bullet = bullet;
        this.direction = direction;
        this.speed = speed;
    }

    public virtual void UpdateBehavior(Vector3Int standingPos)
    {
        Debug.Log("Bullet Move!");
    }
}

// In BaseBehavior class
public class BaseBehavior : ProjectileBehavior
{
    
    public override void UpdateBehavior(Vector3Int standingPos)
    {
        base.UpdateBehavior(standingPos);
        HexCellComponent standingCell = BattleManager.Instance.hexgrid.GetCellInCoord(standingPos);
        HexCellComponent nextCellToMove = BattleManager.Instance.hexgrid.GetCellByDirection(standingCell, direction);
        
        if(nextCellToMove.CellData.CellType != CellType.Invalid || nextCellToMove != null)
        {
            
            this.transform.DOMove(nextCellToMove.transform.position, 1f);
            bullet.StandingPos = nextCellToMove.CellData.Coordinates;
        }
        else
        {
            bullet.SelfDestroy();
            return;
        }
        
    }
}
