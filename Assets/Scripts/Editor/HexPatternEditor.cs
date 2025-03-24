using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(AbilityData))]
public class HexPatternEditor : Editor
{
    private AbilityData abilityData;

    private Type[] hexPatternTypes;

    private int customMatrixSize = 3; // Matrix size input for CustomOffsetPattern

    private Dictionary<Vector2Int, bool> selectionMatrix = new(); // Persistent checkbox state

    private void OnEnable()
    {
        abilityData = (AbilityData)target;

        // Fetch all non-abstract types that implement IHexPatternHelper
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

        DrawHexPatternField("Selectable Pattern", ref abilityData.selectablePattern);
        DrawHexPatternField("AOE Pattern", ref abilityData.aoePattern);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHexPatternField(string label, ref IHexPatternHelper pattern)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        // Identify current type
        Type currentType = pattern?.GetType();

        // Create a dropdown to select pattern type
        int currentIndex = Array.FindIndex(hexPatternTypes, t => t == currentType);
        int selectedIndex = EditorGUILayout.Popup("Pattern Type", currentIndex, hexPatternTypes.Select(t => t.Name).ToArray());

        if (selectedIndex != currentIndex)
        {
            // Change pattern type immediately
            if (selectedIndex >= 0 && selectedIndex < hexPatternTypes.Length)
            {
                pattern = (IHexPatternHelper)Activator.CreateInstance(hexPatternTypes[selectedIndex]);
                EditorUtility.SetDirty(abilityData);
            }
        }

        if (pattern != null)
        {
            // Draw specific UI based on the type of pattern
            EditorGUI.indentLevel++;
            if (pattern is LinePattern linePattern) DrawLinePatternUI(linePattern);
            else if (pattern is HexagonPattern hexagonPattern) DrawHexagonPatternUI(hexagonPattern);
            else if (pattern is TrianglePattern trianglePattern) DrawTrianglePatternUI(trianglePattern);
            else if (pattern is CustomOffsetPattern customOffsetPattern) DrawCustomOffsetPatternUI(customOffsetPattern);
            EditorGUI.indentLevel--;
        }
    }

    private void DrawLinePatternUI(LinePattern pattern)
    {
        pattern.range = EditorGUILayout.IntField("Range", pattern.range);

        // Display multi-select dropdown for HexDirection
        HexDirection[] allDirections = (HexDirection[])Enum.GetValues(typeof(HexDirection));

        bool[] selectedDirections = new bool[allDirections.Length];
        for (int i = 0; i < allDirections.Length; i++)
        {
            if (pattern.dir != null && pattern.dir.Contains(allDirections[i]))
                selectedDirections[i] = true;
        }

        EditorGUILayout.LabelField("Directions");
        for (int i = 0; i < allDirections.Length; i++)
        {
            selectedDirections[i] = EditorGUILayout.Toggle(allDirections[i].ToString(), selectedDirections[i]);
        }

        // Apply the selected directions
        pattern.dir = allDirections.Where((t, i) => selectedDirections[i]).ToArray();
    }

    private void DrawHexagonPatternUI(HexagonPattern pattern)
    {
        pattern.range = EditorGUILayout.IntField("Radius", pattern.range);
    }

    private void DrawTrianglePatternUI(TrianglePattern pattern)
    {
        pattern.iteration = EditorGUILayout.IntField("Iteration", pattern.iteration);
        pattern.isUpward = EditorGUILayout.Toggle("Is Upward", pattern.isUpward);
    }

    private void DrawCustomOffsetPatternUI(CustomOffsetPattern pattern)
    {
        // Ensure Offsets is initialized
        if (pattern.Offsets == null)
        {
            pattern.Offsets = new List<Vector3Int>();
        }

        // Input for matrix size (odd number only)
        customMatrixSize = Mathf.Max(3, EditorGUILayout.IntField("Matrix Size (Odd)", customMatrixSize));

        if (customMatrixSize % 2 == 0)
        {
            customMatrixSize += 1; // Ensure matrix size is always odd
        }

        int center = customMatrixSize / 2;

        // Initialize selectionMatrix from pattern.Offsets
        foreach (var offset in pattern.Offsets)
        {
            Vector2Int key = new(offset.x, offset.z);
            if (!selectionMatrix.ContainsKey(key))
            {
                selectionMatrix[key] = true;
            }
        }

        EditorGUILayout.LabelField("Select Offsets:");

        for (int z = 0; z < customMatrixSize; z++)
        {
            EditorGUILayout.BeginHorizontal();

            // Indent odd rows for hexagonal display
            if (z % 2 == 1)
            {
                GUILayout.Space(12);
            }

            for (int x = 0; x < customMatrixSize; x++)
            {
                Vector2Int offset = new(x - center, z - center);

                // Skip the center cell
                if (x == center && z == center)
                {
                    GUILayout.Space(20);
                    continue;
                }

                bool isChecked = selectionMatrix.ContainsKey(offset) && selectionMatrix[offset];
                bool newValue = GUILayout.Toggle(isChecked, "", GUILayout.Width(20));

                if (newValue)
                    selectionMatrix[offset] = true;
                else
                    selectionMatrix[offset] = false;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Save Offsets"))
        {
            pattern.Offsets.Clear();

            foreach (var p in selectionMatrix)
            {
                if (p.Value && p.Key != Vector2Int.zero) // Skip center
                {
                    pattern.Offsets.Add(new Vector3Int(p.Key.x, 0, p.Key.y));
                }
            }

            EditorUtility.SetDirty(abilityData);
        }
    }
} 
