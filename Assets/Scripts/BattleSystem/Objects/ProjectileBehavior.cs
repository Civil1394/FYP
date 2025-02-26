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
    private  HexCellComponent nextCellToMove = new HexCellComponent();
    public override float UpdateBehavior()
    {
        this.DOKill();
        for (int i = 0; i < lifeTime; i++)
        {
           
            if (!nextCellToMove || nextCellToMove.CellData.CellType == CellType.Invalid)
            {
                break;
            }
            nextCellToMove = BattleManager.Instance.hexgrid.GetCellByDirection(standingCell, direction);
            standingCell = nextCellToMove;
        }
        
        
        Tween currentMovement = this.transform.DOMove(nextCellToMove.transform.position + height_offset, speed)
            .SetEase(Ease.Linear)
            .OnComplete(()=>
            { 
                BulletActor.SelfDestroy();
            });
        return 1;
    }

    private void OnDestroy()
    {
    }
}
