using UnityEngine;
using System.Collections;

public class PlayerDetector : MonoBehaviour 
{
    [SerializeField] float angleOfRange = 60f;
    [SerializeField] float distanceOfRange = 10f;
    [SerializeField] float innerSphereRadius = 3f;
    Transform player;
    Color transparentYellow = new Color(0, 1, 1, 0.25f);
    private void Start()
    {
        player = GameObject.FindObjectOfType<PlayerMovement>().transform;
    }
    public bool CanDetectPlayer(out Transform playerPos)
    {
        var dirToPlayer = player.position - transform.position;
        var angleToPlayer = Vector3.Angle(dirToPlayer,transform.forward);

        if ((angleToPlayer > angleOfRange / 2 || dirToPlayer.magnitude > distanceOfRange)&& dirToPlayer.magnitude > innerSphereRadius)
        {
            playerPos = null;
            return false;
        }
        RaycastHit hit;
        Ray ray = new Ray(transform.position, dirToPlayer);
        Debug.DrawRay(transform.position, dirToPlayer);
        Physics.Raycast(ray, out hit, distanceOfRange);
        var haveDirLineOfSight = hit.collider.CompareTag("Player");
        if (!haveDirLineOfSight)
        {
            playerPos = null;
            return false;
        }
        playerPos = player;
        return true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(-angleOfRange / 2, Vector3.up) * transform.forward * distanceOfRange);
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(angleOfRange / 2, Vector3.up) * transform.forward * distanceOfRange);
        Gizmos.color = transparentYellow;
        Gizmos.DrawSphere(transform.position, innerSphereRadius);
    }
}