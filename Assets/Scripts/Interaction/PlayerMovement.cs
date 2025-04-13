using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gizmoLength = 2f; 
    public Color gizmoColor = Color.blue; 

    private Rigidbody rb;
    private Vector3 movement;
    private Vector3 lastMovementDirection;

    public void ChangeFacingDirection(HexCellComponent targetCell)
    {
        this.transform.DOLookAt(targetCell.CalPosForAction(), 0.2f);
    }
    public void Move(HexCellComponent targetCell)
    {
        this.transform.DOMove(targetCell.CalPosForAction(), 0.5f);
        this.transform.DOLookAt(targetCell.CalPosForAction(), 0.2f);
    }

    public void Dash(HexCellComponent targetCell)
    {
        this.transform.DOMove(targetCell.CalPosForAction(), 0.5f).SetEase(Ease.InBack);
        this.transform.DOLookAt(targetCell.CalPosForAction(), 0.2f);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Vector3 direction = transform.forward * gizmoLength;
        Gizmos.DrawLine(transform.position, transform.position + direction);
        
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawLine(transform.position + direction, transform.position + direction + right * 0.25f);
        Gizmos.DrawLine(transform.position + direction, transform.position + direction + left * 0.25f);
    }


}
