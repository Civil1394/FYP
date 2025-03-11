using UnityEngine;
using DG.Tweening;


public abstract class ProjectileBehavior : MonoBehaviour
{
    protected BulletActor BulletActor;
    protected HexCellComponent standingCell;
    protected HexDirection direction;
    protected float speed;
    protected Vector3 height_offset;
    protected float lifeTime;
    public virtual void Initialize(BulletActor bulletActor, Vector3Int standingPos, HexDirection direction, float speed,
        Vector3 height_offset, float lifeTime)
    {
        this.BulletActor = bulletActor;
        this.standingCell = BattleManager.Instance.hexgrid.GetCellInCoord(standingPos);
        this.direction = direction;
        this.speed = speed;
        this.height_offset = height_offset;
        this.lifeTime = lifeTime;
    }

    public abstract float UpdateBehavior();

}

public class LinearBehavior : ProjectileBehavior
{
    public override float UpdateBehavior()
    {
        HexCellComponent finalDest = standingCell;
        HexCellComponent nextCellToMove = new HexCellComponent();
        
        this.DOKill();
        for (int i = 0; i < lifeTime; i++)
        {
            nextCellToMove = BattleManager.Instance.hexgrid.GetCellByDirection(finalDest, direction);
           
            if (nextCellToMove == null)
            {
                break;
            }
            
            if (nextCellToMove.CellData.CellType == CellType.Invalid)
            {
                break;
            }
            
            finalDest = nextCellToMove;
        }
        
        Vector3 startPos = standingCell.transform.position;
        Vector3 endPos   = finalDest.transform.position + height_offset;
        float distance = Vector3.Distance(startPos, endPos);
        float travelTime = distance / speed;
        
        Tween currentMovement = this.transform.DOMove(endPos, travelTime)
            .SetEase(Ease.Linear)
            .OnComplete(()=>
            { 
                BulletActor.SelfDestroy();
            });
        return 1;
    }
    
}
