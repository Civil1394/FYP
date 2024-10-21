using UnityEngine;

public class HexCellMeshGenerator : MonoBehaviour
{
    public static float OUTER_RADIUS = 0.2f;
    public static float INNER_RADIUS;
    public bool flatTopped = false;
    [SerializeField] private float collideOffset = 1.5f;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = GenerateHexMesh();
        //OnDrawGizmosSelected();
    }

    private Mesh GenerateHexMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Hex";

        Vector3[] vertices = new Vector3[7];
        int[] triangles = new int[18];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            if (flatTopped)
            {
                angle += 30f * Mathf.Deg2Rad;
            }
            vertices[i + 1] = new Vector3(OUTER_RADIUS * Mathf.Cos(angle), 0f, OUTER_RADIUS * Mathf.Sin(angle));
        }

        for (int i = 0; i < 6; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2 > 6 ? 1 : i + 2;
            triangles[i * 3 + 2] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        this.GetComponent<MeshCollider>().sharedMesh = mesh;
        return mesh;
    }

    public static float GetHexagonWidth()
    {
        return GetInnerRadius() * 2f;
    }

    public static float GetInnerRadius()
    {
        INNER_RADIUS = OUTER_RADIUS * 0.866025404f;
        return INNER_RADIUS;
    }
    public static float GetHexArea()
    {
        float area = INNER_RADIUS / 2 * Mathf.PI;
        return area;
    }
    
    public bool CheckForCollisionAtCurrentPosition()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, INNER_RADIUS*collideOffset);
        return colliders.Length > 1; // > 1 because it will detect its own collider
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, INNER_RADIUS*collideOffset);
    }
}