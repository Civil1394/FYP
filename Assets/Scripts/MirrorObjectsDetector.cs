using UnityEngine;
using System.Collections.Generic;

public class MirrorObjectDetector : MonoBehaviour
{
    public Camera mirrorCamera;
    public int horizontalRays = 10;
    public int verticalRays = 10;
    public float rayDistance = 1000f;
    public LayerMask detectionLayers;
    public bool showGizmos = true;
    public Color gizmoColor = Color.yellow;

    [SerializeField] public List<ReflectedObjectData> reflectedObjectsList = new List<ReflectedObjectData>();
    private Dictionary<int, ReflectedObjectData> reflectedObjectsDict = new Dictionary<int, ReflectedObjectData>();
    private MeshCollider mirrorCollider;
    private List<Ray> gizmoRays = new List<Ray>();

    void Start()
    {
        mirrorCollider = GetComponent<MeshCollider>();
        if (mirrorCamera == null)
        {
            mirrorCamera = GameObject.Find("MirrorCamera").GetComponent<Camera>();
        }
        
        DetectReflectedObjects();
        UpdateReflectedObjectsList();
    }

    void Update()
    {
        
    }

  void DetectReflectedObjects()
{
    reflectedObjectsDict.Clear();
    gizmoRays.Clear();

    Vector3 mirrorNormal = -transform.forward;
    Vector3 mirrorRight = transform.right;
    Vector3 mirrorUp = transform.forward;

    // Get the mesh from the MeshFilter component
    Mesh mirrorMesh = GetComponent<MeshFilter>().mesh;
    Vector3[] vertices = mirrorMesh.vertices;

    // Transform vertices to world space
    for (int i = 0; i < vertices.Length; i++)
    {
        vertices[i] = transform.TransformPoint(vertices[i]);
        Debug.Log(vertices[i]);
    }

    // Calculate actual bounds from transformed vertices
    Bounds actualBounds = new Bounds(vertices[0], Vector3.zero);
    for (int i = 1; i < vertices.Length; i++)
    {
        actualBounds.Encapsulate(vertices[i]);
    }

    Vector3 mirrorSize = actualBounds.size;
    Vector3 mirrorCenter = actualBounds.center;
    
    for (int x = 0; x < horizontalRays; x++)
    {
        for (int y = 0; y < verticalRays; y++)
        {
            float xPercent = x / (float)(horizontalRays - 1);
            float yPercent = y / (float)(verticalRays - 1);
            //Debug.Log(verticalRays + " , " +yPercent);
            // Calculate the world position on the mirror plane
            Vector3 rayStart = mirrorCenter + 
                (mirrorRight * (xPercent - 0.5f) * mirrorSize.x) +
                (mirrorUp * (yPercent - 0.5f) * mirrorSize.y);

            //Debug.Log(verticalRays + " , " +rayStart);
            // Calculate direction from mirror camera through the ray start point
            Vector3 cameraToPoint = rayStart - mirrorCamera.transform.position;
            Vector3 rayDirection = cameraToPoint.normalized;

            // Reflect the ray direction off the mirror surface
            Vector3 reflectedDirection = Vector3.Reflect(rayDirection, mirrorNormal);

            // Store the ray for gizmo drawing
            gizmoRays.Add(new Ray(rayStart, reflectedDirection));

            RaycastHit[] hits = Physics.RaycastAll(rayStart, reflectedDirection, rayDistance, detectionLayers);

            foreach (RaycastHit hit in hits)
            {
                int objectId = hit.collider.gameObject.GetInstanceID();
                if (!reflectedObjectsDict.ContainsKey(objectId))
                {
                    reflectedObjectsDict[objectId] = new ReflectedObjectData
                    {
                        GameObject = hit.collider.gameObject,
                        Position = hit.point,
                        Distance = hit.distance
                    };
                }
            }
        }
    }
}

    void UpdateReflectedObjectsList()
    {
        reflectedObjectsList.Clear();
        foreach (var kvp in reflectedObjectsDict)
        {
            reflectedObjectsList.Add(kvp.Value);
        }
    }

    public Dictionary<int, ReflectedObjectData> GetReflectedObjects()
    {
        return reflectedObjectsDict;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || !Application.isPlaying) return;

        Gizmos.color = gizmoColor;

        foreach (Ray ray in gizmoRays)
        {
            Gizmos.DrawRay(ray.origin, ray.direction * rayDistance);
            Gizmos.DrawSphere(ray.origin, 0.02f);
        }

        // Draw detected objects
        Gizmos.color = Color.green;
        foreach (var objData in reflectedObjectsList)
        {
            Gizmos.DrawSphere(objData.Position, 0.1f);
        }

        // Draw the actual mirror bounds
        Gizmos.color = Color.blue;
        if (GetComponent<MeshFilter>() != null)
        {
            Mesh mirrorMesh = GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mirrorMesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = transform.TransformPoint(vertices[i]);
            }
            Bounds actualBounds = new Bounds(vertices[0], Vector3.zero);
            for (int i = 1; i < vertices.Length; i++)
            {
                actualBounds.Encapsulate(vertices[i]);
            }
            Gizmos.DrawWireCube(actualBounds.center, actualBounds.size);
        }
    }
}

[System.Serializable]
public class ReflectedObjectData
{
    public GameObject GameObject;
    public Vector3 Position;
    public float Distance;
}