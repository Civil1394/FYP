using UnityEngine;
using DG.Tweening;

public class DirectionProjectileBehaviour : MonoBehaviour 
{
    public void Init(Vector3 targetPos)
    {
        transform.DOMove(targetPos, 2);
    }
}