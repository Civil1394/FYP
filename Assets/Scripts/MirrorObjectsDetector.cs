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
       
       
    }

    void Update()
    {
        DetectReflectedObjects();
        UpdateReflectedObjectsList();
    }

    void DetectReflectedObjects()
    {
        reflectedObjectsDict.Clear();
        gizmoRays.Clear();

        // Get the mesh from the MeshFilter component
        Mesh mirrorMesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mirrorMesh.vertices;
        Vector3[] normals = mirrorMesh.normals;

        // Transform vertices and normals to world space
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = transform.TransformPoint(vertices[i]);
            normals[i] = transform.TransformDirection(normals[i]);
        }

        // Shoot rays from each vertex
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 rayStart = vertices[i];
            Vector3 rayDirection = normals[i].normalized;

            // Store the ray for gizmo drawing
            gizmoRays.Add(new Ray(rayStart, rayDirection));

            RaycastHit[] hits = Physics.RaycastAll(rayStart, rayDirection, rayDistance, detectionLayers);

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