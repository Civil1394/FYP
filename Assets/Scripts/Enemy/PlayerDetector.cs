using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] float angleOfRange = 60f;
    [SerializeField] float distanceOfRange = 10f;
    [SerializeField] float innerSphereRadius = 3f;
    private bool canSeePlayer = false;
    private HexCellComponent localPlayerGrid;
    private AIBrain enemyBrain;
    Transform player;

    private void Start()
    {
        player = GameObject.FindObjectOfType<PlayerMovement>().transform;
    }

    public void Init(AIBrain aiBrain, float angle, float distance, float innerRadius)
    {
        angleOfRange = angle;
        distanceOfRange = distance;
        innerSphereRadius = innerRadius;
        enemyBrain = aiBrain;
    }
    void Update()
    {
        DetectPlayer();
    }

    public bool CanDetectPlayer(out HexCellComponent playerGrid)
    {
        playerGrid = localPlayerGrid;
        return canSeePlayer;
    }
    public void DetectPlayer()
    {
        var playerGrid = BattleManager.Instance.GetPlayerCell().CellData;

        var dirToPlayer = player.position - transform.position;
        var angleToPlayer = Vector3.Angle(dirToPlayer, transform.forward);
        float gridDisToPlayer = Vector3.Distance(enemyBrain.currentCell.Coordinates, playerGrid.Coordinates);
        if ((angleToPlayer > angleOfRange / 2 || gridDisToPlayer > distanceOfRange) && gridDisToPlayer > innerSphereRadius)
        {
            localPlayerGrid = null;
            canSeePlayer = false;
            return;
        }

        if (!CheckLineOfSight(dirToPlayer))
        {
            localPlayerGrid = null;
            canSeePlayer = false;
            return;
        }
        localPlayerGrid = BattleManager.Instance.GetPlayerCell();
        canSeePlayer = true;
    }
    public bool CheckLineOfSight(Vector3 dirToPlayer)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, dirToPlayer);
        Debug.DrawRay(transform.position, dirToPlayer);
    
        // Check if the raycast hits anything
        if (Physics.Raycast(ray, out hit))
        {
            // Only check the tag if we actually hit something
            return hit.collider.CompareTag("Player");
        }
    
        // Return false if the ray didn't hit anything
        return false;
    }
}