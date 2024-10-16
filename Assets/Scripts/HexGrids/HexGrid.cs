using UnityEngine;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour
{
    public HexMeshGenerator hexPrefab;
    public int numberOfHexagons = 5;

    void Start()
    {
        CreateHexagonRow();
    }

    void CreateHexagonRow()
    {
        float hexWidth = hexPrefab.GetHexagonWidth();
        Debug.Log(hexWidth);
        for (int i = 0; i < numberOfHexagons; i++)
        {
            HexMeshGenerator hex = Instantiate(hexPrefab, transform);
            float xPosition = i * hexWidth;
            hex.transform.localPosition = new Vector3(xPosition, 0.1f, 0f);
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