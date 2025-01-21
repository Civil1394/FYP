using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] float angleOfRange = 60f;
    [SerializeField] float distanceOfRange = 10f;
    [SerializeField] float innerSphereRadius = 3f;
    bool canSeePlayer = false;
    private HexCellComponent localPlayerGrid;
    Transform player;
    Color transparentYellow = new Color(0, 1, 1, 0.25f);
    private void Start()
    {
        player = GameObject.FindObjectOfType<PlayerMovement>().transform;
    }

    void Update()
    {
        canSeePlayer = DetectPlayer();
    }

    public bool CanDetectPlayer(out HexCellComponent playerGrid)
    {
        playerGrid = localPlayerGrid;
        return canSeePlayer;
    }
    public bool DetectPlayer()
    {
        var dirToPlayer = player.position - transform.position;
        var angleToPlayer = Vector3.Angle(dirToPlayer, transform.forward);

        if ((angleToPlayer > angleOfRange / 2 || dirToPlayer.magnitude > distanceOfRange) && dirToPlayer.magnitude > innerSphereRadius)
        {
            localPlayerGrid = null;
            return false;
        }

        if (!CheckLineOfSight(dirToPlayer))
        {
            localPlayerGrid = null;
            return false;
        }
        localPlayerGrid = BattleManager.Instance.GetPlayerCell();
        return true;
    }
    public bool CheckLineOfSight(Vector3 dirToPlayer)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, dirToPlayer);
        Debug.DrawRay(transform.position, dirToPlayer);
        Physics.Raycast(ray, out hit, distanceOfRange);
        return hit.collider.CompareTag("Player");
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