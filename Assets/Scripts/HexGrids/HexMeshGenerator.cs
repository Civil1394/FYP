using UnityEngine;

public class HexMeshGenerator : MonoBehaviour
{
    public float outerRadius = 1f;
    public float innerRadius;
    public bool flatTopped = false;

    private void Awake()
    {
        innerRadius = outerRadius * 0.866025404f; // Calculate innerRadius in Awake
        GetComponent<MeshFilter>().mesh = GenerateHexMesh();
        OnDrawGizmosSelected();
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
            vertices[i + 1] = new Vector3(outerRadius * Mathf.Cos(angle), 0f, outerRadius * Mathf.Sin(angle));
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

        //this.gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        return mesh;
    }

    public float GetHexagonWidth()
    {
        if (flatTopped)
        {
            // For flat-topped hexagons, the width is 2 * outerRadius
            return innerRadius * 2f;
        }
        else
        {
            // For pointy-topped hexagons, the width is 2 * innerRadius
            return 2f * outerRadius;
        }
    }
    
    public bool CheckForCollisionAtCurrentPosition()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, innerRadius );
        return colliders.Length > 1; // > 1 because it will detect its own collider
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, innerRadius);
    }
}