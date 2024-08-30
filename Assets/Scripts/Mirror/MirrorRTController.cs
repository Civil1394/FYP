using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class MirrorRTController : MonoBehaviour
{
    
    [SerializeField] private Camera mirrorCamera;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private int textureSize = 1024;
    private Material mtlRenderTexture;

    private void OnEnable()
    {
        CreateRenderTexture();
    }

    private void OnDisable()
    {
        CleanupRenderTexture();
    }

    private void CreateRenderTexture()
    {
        CleanupRenderTexture(); 

        renderTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
        renderTexture.name = "Mirror RenderTexture " + GetInstanceID();
        renderTexture.dimension = TextureDimension.Tex2D;
        renderTexture.antiAliasing = 1; // None
        renderTexture.useMipMap = false;
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.filterMode = FilterMode.Bilinear;

        if (mirrorCamera != null)
        {
            mirrorCamera.targetTexture = renderTexture;
        }
        else
        {
            Debug.LogError("Mirror camera is not assigned!", this);
        }

        mtlRenderTexture = new Material(Shader.Find("Unlit/Texture"));
        mtlRenderTexture.mainTexture = renderTexture;

        Renderer mirrorRenderer = GetComponent<Renderer>();
        if (mirrorRenderer != null)
        {
            mirrorRenderer.material = mtlRenderTexture;
        }
    }

    private void CleanupRenderTexture()
    {
        // unassign the RenderTexture from the camera
        if (mirrorCamera != null)
        {
            mirrorCamera.targetTexture = null;
        }

        // safe to release and destroy the RenderTexture
        if (renderTexture != null)
        {
            renderTexture.Release();
            DestroyImmediate(renderTexture);
            renderTexture = null;
        }

        if (mtlRenderTexture != null)
        {
            DestroyImmediate(mtlRenderTexture);
            mtlRenderTexture = null;
        }

        // Reset the renderer's material to avoid null reference
        Renderer mirrorRenderer = GetComponent<Renderer>();
        if (mirrorRenderer != null)
        {
            mirrorRenderer.material = null;
        }
    }

    private void OnValidate()
    {
        CreateRenderTexture();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MirrorRTController))]
    public class MirrorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MirrorRTController mirror = (MirrorRTController)target;
            if (mirror.renderTexture != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("RenderTexture Preview");
                EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(100, 300), mirror.renderTexture);
            }
        }
    }
#endif
}