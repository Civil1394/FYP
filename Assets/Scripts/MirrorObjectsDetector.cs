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

    Vector3 mirrorNormal = -transform.forward;
    Vector3 mirrorRight = transform.right;
    Vector3 mirrorUp = transform.up;

    Bounds mirrorBounds = mirrorCollider.bounds;
    Vector3 mirrorSize = mirrorBounds.size;

    for (int x = 0; x < horizontalRays; x++)
    {
        for (int y = 0; y < verticalRays; y++)
        {
            float xPercent = x / (float)(horizontalRays - 1);
            float yPercent = y / (float)(verticalRays - 1);

            // Calculate the local position on the mirror plane
            Vector3 localPos = new Vector3(
                Mathf.Lerp(-mirrorSize.x * 0.5f, mirrorSize.x * 0.5f, xPercent),
                Mathf.Lerp(-mirrorSize.y * 0.5f, mirrorSize.y * 0.5f, yPercent),
                0
            );
            Debug.Log(y + " , " + localPos);
            // Transform the local position to world space
            Vector3 rayStart = transform.TransformPoint(localPos);
            Debug.Log(y + " , " + rayStart);
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
            // Draw a small sphere at the ray origin for better visibility
            Gizmos.DrawSphere(ray.origin, 0.02f);
        }

        // Draw detected objects
        Gizmos.color = Color.green;
        foreach (var objData in reflectedObjectsList)
        {
            Gizmos.DrawSphere(objData.Position, 0.1f);
        }

        // Draw the mirror plane
        Gizmos.color = Color.blue;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, mirrorCollider.bounds.size);
        Gizmos.matrix = Matrix4x4.identity;
    }
}

[System.Serializable]
public class ReflectedObjectData
{
    public GameObject GameObject;
    public Vector3 Position;
    public float Distance;
}