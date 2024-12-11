using UnityEngine;
using DG.Tweening;

public class DirectionProjectileBehaviour : MonoBehaviour 
{
    int distance = 20;
    public void Init(Vector3 dir)
    {
        Debug.DrawLine(transform.position, dir * distance + transform.position, Color.red,1);
        transform.DOMove(dir * distance+transform.position, 2)
            .OnComplete(()=>Destroy(gameObject));
    }
}