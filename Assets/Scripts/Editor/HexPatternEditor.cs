using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AbilityData))]
public class HexPatternEditor : Editor
{
    [SerializeField] private int gridSize = 13; // Size of the grid (will be a gridSize x gridSize matrix)
    private const float CELL_SIZE = 20f; // Size of each cell in the grid
    private const float CENTER_OFFSET = CELL_SIZE * 3 / 4f; // Offset for the center row

    private bool[,] checkboxMatrix;
    private Vector2 scrollPosition;
    
    private SerializedProperty customOffsetPatternProp;
    private SerializedProperty offsetsProp;
    private void OnEnable()
    {
        // Find the property that holds the CustomOffsetPattern
        // This assumes your MonoBehaviour has a field of type CustomOffsetPattern
        // Adjust the property path as needed for your specific implementation
        FindCustomOffsetPatternProperty();

        // Initialize the checkbox matrix
        checkboxMatrix = new bool[gridSize, gridSize];
        
        // If we have existing offsets, populate the matrix
        if (customOffsetPatternProp != null)
        {
            PopulateMatrixFromOffsets();
        }
    }

    private void FindCustomOffsetPatternProperty()
    {
        customOffsetPatternProp = serializedObject.FindProperty("pattern");
        offsetsProp = customOffsetPatternProp.FindPropertyRelative("Offsets");
    }

    private void PopulateMatrixFromOffsets()
    {
        // Clear the matrix first
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                checkboxMatrix[x, z] = false;
            }
        }

        // Get the center of the grid
        int centerX = gridSize / 2;
        int centerZ = gridSize / 2;

        // Get the actual abilityData component
        AbilityData abilityData = (AbilityData)target;
        if (abilityData.pattern == null)
        {
            return;
        }
        var offsets = abilityData.pattern.Offsets;
        if (offsets != null)
        {
            foreach (var offset in offsets)
            {
                // Convert offset coordinates to matrix indices
                int matrixX = centerX + offset.x;
                int matrixZ = centerZ + offset.z;

                // Check if this position is within bounds of our matrix
                if (matrixX >= 0 && matrixX < gridSize && matrixZ >= 0 && matrixZ < gridSize)
                {
                    checkboxMatrix[matrixX, matrixZ] = true;
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // If we couldn't find the CustomOffsetPattern property, just return
        
        if (customOffsetPatternProp == null)
        {
            EditorGUILayout.HelpBox("No CustomOffsetPattern field found.", MessageType.Warning);
            return;
        }
        
        EditorGUILayout.LabelField("Hex Offset Pattern", EditorStyles.boldLabel);
        
        serializedObject.Update();
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        // Draw the hex grid UI
        DrawCustomInput();
        
        EditorGUILayout.EndScrollView();
        
        // Apply button
        if (GUILayout.Button("Apply Pattern"))
        {
            ApplyPatternFromMatrix();
            serializedObject.ApplyModifiedProperties();
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawCustomInput()
    {
        int centerX = gridSize / 2;
        int centerZ = gridSize / 2;
        
        for (int z = 0; z < gridSize; z++)
        {
            GUILayout.BeginHorizontal();
            
            // Indent odd rows (in a hex grid) to create the hex effect
            if (z % 2 == 1)
            {
                GUILayout.Space(CENTER_OFFSET);
            }

            for (int x = 0; x < gridSize; x++)
            {
                // Calculate the offset coordinates
                int offsetX = x - centerX;
                int offsetZ = z - centerZ;

                // Create a tooltip showing the coordinates
                string tooltip = $"Offset: ({offsetX}, {offsetZ})";
                
                // Special styling for the center cell
                GUIStyle style = new GUIStyle(GUI.skin.toggle);
                if (x == centerX && z == centerZ)
                {
                    GUILayout.Space(CELL_SIZE);
                    continue;
                    // style.normal.textColor = Color.red;
                    // style.onNormal.textColor = Color.red;
                    // tooltip = "Center (0, 0, 0)";
                }

                EditorGUI.BeginChangeCheck();
                bool newValue = GUILayout.Toggle(checkboxMatrix[x, z], new GUIContent("", tooltip), style, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                if (EditorGUI.EndChangeCheck())
                {
                    checkboxMatrix[x, z] = newValue;
                }
            }
            
            GUILayout.EndHorizontal();
        }
    }

    private void DrawLinePatternInput()
    {
        
    }
    private void ApplyPatternFromMatrix()
    {
        // Clear the existing offsets
        offsetsProp.ClearArray();
        
        // Get the center of the grid
        int centerX = gridSize / 2;
        int centerZ = gridSize / 2;
        
        // Convert matrix to offsets
        List<Vector3Int> newOffsets = new List<Vector3Int>();
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (checkboxMatrix[x, z])
                {
                    // Skip the center cell since it's the reference cell
                    if (x == centerX && z == centerZ)
                        continue;
                        
                    // Convert matrix indices to offset coordinates
                    int offsetX = x - centerX;
                    int offsetZ = z - centerZ;
                    
                    newOffsets.Add(new Vector3Int(offsetX, 0, offsetZ));
                }
            }
        }
        
        // Add new elements to the array
        offsetsProp.arraySize = newOffsets.Count;
        for (int i = 0; i < newOffsets.Count; i++)
        {
            SerializedProperty offsetElement = offsetsProp.GetArrayElementAtIndex(i);
            offsetElement.FindPropertyRelative("x").intValue = newOffsets[i].x;
            offsetElement.FindPropertyRelative("y").intValue = newOffsets[i].y;
            offsetElement.FindPropertyRelative("z").intValue = newOffsets[i].z;
        }
    }
}