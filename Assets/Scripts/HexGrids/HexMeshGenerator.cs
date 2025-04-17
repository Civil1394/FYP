using UnityEngine;

public class HexCellMeshGenerator : MonoBehaviour
{
    public static float OUTER_RADIUS = 0.17f;
    public static float INNER_RADIUS;
    public bool flatTopped = false;
    [SerializeField] private float collideOffset = 1.5f;

    [SerializeField] private float outlineWidth = 0.05f;
    [SerializeField] private Color outlineColor = Color.black;
    [SerializeField] private Material outlineMaterial; // Reference to URP Unlit material
    [SerializeField] private bool isAddOutline = true;
    private void Awake()
    {
        Mesh mesh = GenerateHexMesh();
        GetComponent<MeshFilter>().mesh = mesh;
        if(isAddOutline) AddOutline(mesh.vertices); // Add outline after generating the mesh
    }
    
    private void AddOutline(Vector3[] vertices)
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        if (lr == null)
        {
            lr = gameObject.AddComponent<LineRenderer>();
        }

        // Extract outer vertices (indices 1-6)
        Vector3[] outerVertices = new Vector3[6];
        for (int i = 0; i < 6; i++)
        {
            outerVertices[i] = vertices[i + 1];
        }

        // Configure LineRenderer
        lr.useWorldSpace = false; // Use local space
        lr.loop = true; // Close the loop
        lr.positionCount = 6;
        lr.SetPositions(outerVertices);

        // Set appearance
        lr.material = new Material(outlineMaterial.shader);
        lr.material.SetFloat("_Surface", 1); // Optional: for transparency
        lr.material.SetColor("_BaseColor", outlineColor);
        lr.startWidth = outlineWidth;
        lr.endWidth = outlineWidth;
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

        if (this.TryGetComponent(out MeshCollider meshCollider))
        {
            meshCollider.sharedMesh = mesh;
        }
        //this.GetComponent<MeshCollider>().sharedMesh = mesh;
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, INNER_RADIUS*collideOffset);
    }
}