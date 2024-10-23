using UnityEngine;

public class AISensor : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] float angle;
    [SerializeField] float height;
    Vector3 playerPos;
    MeshCollider meshCollider;
    Mesh mesh;
    Color redColor = new Color(1, 0, 0, 0.25f);

    public bool haveDirLineOfSightToPlayer = false;
    private void Start()
    {
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        if (mesh)
        {
            meshCollider.sharedMesh = mesh;
        }
        else
        {
            meshCollider.sharedMesh = CreateWedgeMesh();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RaycastHit hit;
            var dir = other.transform.position - (transform.position + Vector3.up * 2);
            Physics.Raycast(transform.position + Vector3.up, dir, out hit, distance);
            Debug.DrawRay(transform.position + Vector3.up, dir, Color.green);
            if (!hit.collider)
            {
                haveDirLineOfSightToPlayer = false;
            }
            else
            {
                print("have direct line of sight to player");
                haveDirLineOfSightToPlayer = hit.transform.CompareTag("Player");
            }
        }
    }

    public bool CanDetectPlayer(out Vector3 PlayerPos)
    {
        PlayerPos = playerPos;
        return haveDirLineOfSightToPlayer;
    }
    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();
        int numOfSegments = 8;
        int numOfTriangle = (numOfSegments * 4) + 4;
        int numOfVertices = numOfTriangle * 3;
        Vector3[] vertices = new Vector3[numOfVertices];
        int[] triangles = new int[numOfVertices];
        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;
        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int vertIndex = 0;

        //left
        vertices[vertIndex++] = bottomCenter;
        vertices[vertIndex++] = bottomLeft;
        vertices[vertIndex++] = topLeft;

        vertices[vertIndex++] = topLeft;
        vertices[vertIndex++] = topCenter;
        vertices[vertIndex++] = bottomCenter;
        //right
        vertices[vertIndex++] = bottomCenter;
        vertices[vertIndex++] = topCenter;
        vertices[vertIndex++] = topRight;

        vertices[vertIndex++] = topRight;
        vertices[vertIndex++] = bottomRight;
        vertices[vertIndex++] = bottomCenter;
        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / numOfSegments;
        for (int i = 0; i < numOfSegments; i++)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;
            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;
            //forward
            vertices[vertIndex++] = bottomLeft;
            vertices[vertIndex++] = bottomRight;
            vertices[vertIndex++] = topRight;

            vertices[vertIndex++] = topRight;
            vertices[vertIndex++] = topLeft;
            vertices[vertIndex++] = bottomLeft;
            //top
            vertices[vertIndex++] = topCenter;
            vertices[vertIndex++] = topLeft;
            vertices[vertIndex++] = topRight;
            //bottom
            vertices[vertIndex++] = bottomCenter;
            vertices[vertIndex++] = bottomRight;
            vertices[vertIndex++] = bottomLeft;
            currentAngle += deltaAngle;
        }



        for (int i = 0; i < numOfVertices; i++)
        {
            triangles[i] = i;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
    }
    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = redColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }
    }
}

