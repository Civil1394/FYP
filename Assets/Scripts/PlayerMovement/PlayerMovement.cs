using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(InputHandler))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gizmoLength = 2f; 
    public Color gizmoColor = Color.blue; 

    private Rigidbody rb;
    private Vector3 movement;
    private Vector3 lastMovementDirection;
    private InputHandler inputHandler;
    void Start()
    {

        BattleManager.Instance.OnPlayerMove += Move;
    }

    private void Move(HexCellComponent targetCell)
    {

        this.transform.DOMove(targetCell.CalPosForAction(), 0.5f);
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

    private void OnDestroy()
    {
        BattleManager.Instance.OnPlayerMove -= Move;
    }
}
