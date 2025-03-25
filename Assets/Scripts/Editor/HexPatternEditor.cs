using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(AbilityData))]
public class HexPatternEditor : Editor
{
    private AbilityData abilityData;

    private Type[] hexPatternTypes;

    private Dictionary<Vector2Int, bool> selectionMatrix = new();
    private int customMatrixSize = 3; // Matrix size input for CustomOffsetPattern
    private void OnEnable()
    {
        abilityData = (AbilityData)target;

        // Get all scriptable object types implementing IHexPatternHelper
        hexPatternTypes = new Type[]
        {
            typeof(LinePattern),
            typeof(HexagonPattern),
            typeof(TrianglePattern),
            typeof(CustomOffsetPattern)
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw default fields for AbilityData
        DrawDefaultInspector();
        if (abilityData.CastType == AbilityCastType.Location_targeted)
        {
            // Draw pattern fields with proper handling for ScriptableObjects
            DrawHexPatternField("Selectable Pattern", ref abilityData.selectablePattern);
            DrawHexPatternField("AOE Pattern", ref abilityData.aoePattern);
        }
       

        if (GUILayout.Button("Force Save"))
        {
            EditorUtility.SetDirty(abilityData);
            AssetDatabase.SaveAssets();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHexPatternField(string label, ref HexPatternBase pattern)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        // Get the current pattern type
        Type currentType = pattern != null ? pattern.GetType() : null;

        // Create a dropdown to switch pattern types
        int currentIndex = Array.FindIndex(hexPatternTypes, t => t == currentType);
        int selectedIndex = EditorGUILayout.Popup("Pattern Type", currentIndex, hexPatternTypes.Select(t => t.Name).ToArray());

        // Handle pattern type switching
        if (selectedIndex != currentIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < hexPatternTypes.Length)
            {
                // Replace the existing pattern with a new instance
                pattern = CreatePatternInstance(hexPatternTypes[selectedIndex]);
                EditorUtility.SetDirty(abilityData);
            }
        }

        // Draw the specific UI for the selected pattern
        if (pattern != null)
        {
            EditorGUI.indentLevel++;
            if (pattern is LinePattern linePattern) DrawLinePatternUI(linePattern);
            else if (pattern is HexagonPattern hexagonPattern) DrawHexagonPatternUI(hexagonPattern);
            else if (pattern is TrianglePattern trianglePattern) DrawTrianglePatternUI(trianglePattern);
            else if (pattern is CustomOffsetPattern customOffsetPattern) DrawCustomOffsetPatternUI(customOffsetPattern);
            EditorGUI.indentLevel--;
        }
    }

    // Helper: Create and save a new ScriptableObject instance
    private HexPatternBase CreatePatternInstance(Type patternType)
    {
        // Define the asset path using the AbilityData name
        string path = "Assets/Resources/AbilityData/Patterns";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets/Resources/AbilityData", "Patterns");
        }

        // Construct a unique path based on AbilityData name and pattern type
        string assetPath = $"{path}/{abilityData.name}_{patternType.Name}.asset";

        // Check if the asset already exists
        HexPatternBase existingPattern = AssetDatabase.LoadAssetAtPath<HexPatternBase>(assetPath);
        if (existingPattern != null)
        {
            return existingPattern; // Return the existing asset
        }

        // Create a new ScriptableObject if not found
        HexPatternBase newPattern = (HexPatternBase)ScriptableObject.CreateInstance(patternType);
        AssetDatabase.CreateAsset(newPattern, assetPath);
        AssetDatabase.SaveAssets();

        return newPattern;
    }

    private void DrawLinePatternUI(LinePattern pattern)
    {
        pattern.range = EditorGUILayout.IntField("Range", pattern.range);

        // Multi-select dropdown for HexDirection
        HexDirection[] allDirections = (HexDirection[])Enum.GetValues(typeof(HexDirection));
        bool[] selectedDirections = allDirections.Select(dir => pattern.dir?.Contains(dir) ?? false).ToArray();

        EditorGUILayout.LabelField("Directions");
        for (int i = 0; i < allDirections.Length; i++)
        {
            selectedDirections[i] = EditorGUILayout.Toggle(allDirections[i].ToString(), selectedDirections[i]);
        }

        pattern.dir = allDirections.Where((t, i) => selectedDirections[i]).ToArray();

        MarkObjectDirty(pattern);
    }

    private void DrawHexagonPatternUI(HexagonPattern pattern)
    {
        pattern.range = EditorGUILayout.IntField("Radius", pattern.range);
        MarkObjectDirty(pattern);
    }

    private void DrawTrianglePatternUI(TrianglePattern pattern)
    {
        pattern.iteration = EditorGUILayout.IntField("Iteration", pattern.iteration);
        pattern.isUpward = EditorGUILayout.Toggle("Is Upward", pattern.isUpward);
        MarkObjectDirty(pattern);
    }

private void DrawCustomOffsetPatternUI(CustomOffsetPattern pattern)
{
    if (pattern.Offsets == null)
    {
        pattern.Offsets = new System.Collections.Generic.List<Vector3Int>();
    }

    // Allow the user to adjust the grid size (always odd for symmetry)
    customMatrixSize = Mathf.Max(3, EditorGUILayout.IntField("Matrix Size (Odd)", customMatrixSize));
    if (customMatrixSize % 2 == 0) customMatrixSize += 1; // Ensure odd size

    int center = customMatrixSize / 2; // Find center index

    // Sync selectionMatrix with pattern.Offsets
    foreach (var offset in pattern.Offsets)
    {
        Vector2Int key = new(offset.x, offset.z);
        if (!selectionMatrix.ContainsKey(key))
        {
            selectionMatrix[key] = true;
        }
    }

    EditorGUILayout.LabelField("Select Offsets:");

    // Draw the checkbox grid
    for (int z = 0; z < customMatrixSize; z++)
    {
        EditorGUILayout.BeginHorizontal();

        // Indent for odd rows (hex grid style)
        if (z % 2 == 1)
        {
            GUILayout.Space(12);
        }

        for (int x = 0; x < customMatrixSize; x++)
        {
            Vector2Int offset = new(x - center, z - center);

            // Skip the center cell (player position)
            if (x == center && z == center)
            {
                GUILayout.Space(20);
                continue;
            }

            // Ensure the cell exists in the matrix
            if (!selectionMatrix.ContainsKey(offset))
            {
                selectionMatrix[offset] = false;
            }

            // Display a toggle (checkbox) for the cell
            bool newValue = GUILayout.Toggle(selectionMatrix[offset], "", GUILayout.Width(20));

            // Update the matrix if changed
            if (newValue != selectionMatrix[offset])
            {
                selectionMatrix[offset] = newValue;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    // Button to save the selected offsets
    if (GUILayout.Button("Save Offsets"))
    {
        pattern.Offsets.Clear();

        foreach (var entry in selectionMatrix)
        {
            if (entry.Value && entry.Key != Vector2Int.zero) // Skip center
            {
                pattern.Offsets.Add(new Vector3Int(entry.Key.x, 0, entry.Key.y));
            }
        }

        MarkObjectDirty(pattern);
    }
}

    // Ensure the pattern and abilityData are marked as dirty
    private void MarkObjectDirty(UnityEngine.Object obj)
    {
        EditorUtility.SetDirty(obj);
        EditorUtility.SetDirty(abilityData);
    }
}
