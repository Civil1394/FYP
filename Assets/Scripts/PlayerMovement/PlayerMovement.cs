using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gizmoLength = 2f; // Length of the forward direction gizmo
    public Color gizmoColor = Color.blue; // Color of the gizmo

    private Rigidbody rb;
    private Vector3 movement;
    private Vector3 lastMovementDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on the player object!");
        }
        else
        {
            rb.velocity = Vector3.zero; // Ensure no initial velocity
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        // Get input
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        // Calculate movement vector
        movement = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        // Convert movement to isometric direction
        movement = Quaternion.Euler(0, 45, 0) * movement;
    }

    void FixedUpdate()
    {
        // Apply movement
        if (movement != Vector3.zero)
        {
            // Move the player
            Vector3 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            // Rotate the player to face the movement direction
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime));

            lastMovementDirection = movement;
        }
        else if (lastMovementDirection != Vector3.zero)
        {
            // If not moving but we have a last movement direction, keep facing that direction
            Quaternion toRotation = Quaternion.LookRotation(lastMovementDirection, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    void OnDrawGizmos()
    {
        // Draw a line showing the forward direction of the player
        Gizmos.color = gizmoColor;
        Vector3 direction = transform.forward * gizmoLength;
        Gizmos.DrawLine(transform.position, transform.position + direction);

        // Draw an arrowhead
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawLine(transform.position + direction, transform.position + direction + right * 0.25f);
        Gizmos.DrawLine(transform.position + direction, transform.position + direction + left * 0.25f);
    }
}
