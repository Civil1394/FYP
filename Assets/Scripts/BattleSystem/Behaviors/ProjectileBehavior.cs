using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using JetBrains.Annotations;


public abstract class ProjectileBehavior : MonoBehaviour
{
    public enum BehaviorType
    {
        Linear,
        Parabola,
    }
    protected ProjectileActor ProjectileActor;
    protected HexCellComponent casterCell;
    protected HexDirection castingDirection;
    protected float speed;
    protected Vector3 height_offset;
    protected float lifeTime;

    public virtual void Init(ProjectileActor projectileActor, Vector3Int standingPos, HexDirection direction, float speed,
        Vector3 height_offset, float lifeTime)
    {
        this.ProjectileActor = projectileActor;
        this.casterCell = BattleManager.Instance.hexgrid.GetCellInCoord(standingPos);
        this.castingDirection = direction;
        this.speed = speed;
        this.height_offset = height_offset;
        this.lifeTime = lifeTime;
    }

    public abstract float UpdateBehavior();

}

public class LinearProjectileBehavior : ProjectileBehavior
{
    public override float UpdateBehavior()
    {
        HexCellComponent finalDest = casterCell;
        HexCellComponent nextCellToMove = new HexCellComponent();
        
        this.DOKill();
        for (int i = 0; i < lifeTime; i++)
        {
            nextCellToMove = BattleManager.Instance.hexgrid.GetCellByDirection(finalDest, castingDirection);
           
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
        
        Vector3 startPos = casterCell.transform.position;
        Vector3 endPos   = finalDest.transform.position + height_offset;
        float distance = Vector3.Distance(startPos, endPos);
        float travelTime = distance / speed;
        
        this.transform.DOMove(endPos, travelTime)
            .SetEase(Ease.Linear)
            .OnComplete(()=>
            { 
                Destroy(this.gameObject);
            }); 
        return 1;
    }
}

public class ParabolaProjectileBehavior : ProjectileBehavior
{
    public float arcHeight = 20f;
    private GameObject explosiveVFX;
    private Action<GameObject> explosiveVFXCallback;

    private GameObject parabolaBlastVFX;
    private float parabolaBlastDamage;
    private CasterType casterType;

    public void InitParabolaBlast(GameObject vfx, float blastDamage,CasterType casterType,Action<GameObject> statusEffectCallback)
    {
        parabolaBlastVFX = vfx;
        parabolaBlastDamage = blastDamage;
        this.casterType = casterType;
        explosiveVFXCallback = statusEffectCallback;
    }
    public override float UpdateBehavior()
    {
        HexCellComponent finalDest = casterCell;
        
        // Find the final destination cell
        for (int i = 0; i < lifeTime; i++)
        {
            HexCellComponent nextCell = BattleManager.Instance.hexgrid.GetCellByDirection(finalDest, castingDirection);
            if (nextCell == null)
            {
                break;
            }
            
            if (nextCell.CellData.CellType == CellType.Invalid)
            {
                // Instead of breaking, continue searching in the same direction
                // until we find a valid cell or reach the end
                while (nextCell != null && nextCell.CellData.CellType == CellType.Invalid)
                {
                    nextCell = BattleManager.Instance.hexgrid.GetCellByDirection(nextCell, castingDirection);
                }
                if (nextCell == null)
                {
                    break;
                }
            }
            
            finalDest = nextCell;
        }

        Vector3 startPos = casterCell.transform.position + height_offset;
        Vector3 endPos = finalDest.transform.position + height_offset;
        float distance = Vector3.Distance(startPos, endPos);
        float travelTime = distance / speed;
        
        StartCoroutine(MoveInParabola(startPos, endPos, travelTime));

        return travelTime; // Return the total travel time
    }

    private IEnumerator MoveInParabola(Vector3 start, Vector3 end, float time)
    {
        float elapsedTime = 0;
        Vector3 direction = end - start;
        Vector3 midPoint = (start + end) / 2f;
        Vector3 arcVector = Vector3.up * arcHeight;

        while (elapsedTime < time)
        {
            float t = elapsedTime / time;
            Vector3 position = Vector3.Lerp(start, end, t);

            // Calculate the parabolic arc
            position += arcVector * (1 - 4 * (t - 0.5f) * (t - 0.5f));

            transform.position = position;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = end;
      
        GameObject explosiveObject = Instantiate(parabolaBlastVFX, end, Quaternion.identity);
        var ea = explosiveObject.AddComponent<ExplosiveActor>();
         ea.Init(parabolaBlastDamage);
         ea.CasterType = CasterType.Environment;
         ea.OnHitApplyStatusEffect += explosiveVFXCallback;
         yield return new WaitForSeconds(0.5f);
         Destroy(gameObject);
    }
}