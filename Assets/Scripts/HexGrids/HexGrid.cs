using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class HexGrid : MonoBehaviour
{
    public HexMeshGenerator hexPrefab;
    public int numberInRow = 5;
    public int numberOfColumn = 5;

    void Start()
    {
        CreateHexagonRow();
        
    }

    void CreateHexagonRow()
    {
        for (int j = 0; j < numberOfColumn; j++)
        {
            for (int i = 0; i < numberInRow; i++)
            {
                HexMeshGenerator hex = Instantiate(hexPrefab, transform);
                
                float hexWidth = hex.GetHexagonWidth();
                float zPosition = j * (hex.innerRadius + hex.outerRadius/2);
                if(j%2 == 0)hex.transform.localPosition = new Vector3(i*hexWidth, 0.1f, zPosition);
                if(j%2 == 1)hex.transform.localPosition = new Vector3(i*hexWidth + hexWidth/2, 0.1f, zPosition);
                hex.gameObject.name = "HexCell ( " + i + " , " + j + " )";
                // Check for collision and destroy if necessary
                if (hex.CheckForCollisionAtCurrentPosition())
                {
                    Debug.Log($"Hex at position { ( i , j ) } overlaps with another object. Destroying.");
                    Destroy(hex.gameObject);
                    continue;
                }
            } 
        }
        
    }

  
    private void Awake()
    {
        
    }

    private void CreateCell(int x, int z, int i)
    {

        
    }

    // public HexCell GetCell(Vector3Int coordinates)
    // {
    //
    // }
    //
    // public HexCell GetCell(int xOffset, int zOffset)
    // {
    //     
    // }
    
    // public void FitToSurface(Collider surfaceCollider)
    // {
    //     Bounds bounds = surfaceCollider.bounds;
    //
    //     // Calculate the number of cells that can fit in the surface
    //     float cellWidth = HexSize * 1.732f;
    //     float cellHeight = HexSize * 1.5f;
    //
    //     Width = Mathf.FloorToInt(bounds.size.x / cellWidth);
    //     Height = Mathf.FloorToInt(bounds.size.z / cellHeight);
    //
    //     // Recenter the grid
    //     transform.position = bounds.center - new Vector3(Width * cellWidth * 0.5f, 0f, Height * cellHeight * 0.5f);
    //
    //     // Recreate the grid with new dimensions
    //     Awake();
    // }
}