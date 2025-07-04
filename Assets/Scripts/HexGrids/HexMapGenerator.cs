using UnityEngine;
using System.Linq;

public class HexMapGenerator : MonoBehaviour
{
    public HexCellComponent hexCellPrefab;
    public float zOffset = 0.04f;
    
    
    void Start()
    {
        //Debug.Log(GetRightBottomCorner(this.GetComponent<MeshFilter>().mesh, this.transform));
        //CreateHexagonRow();
        AutoGenFullCoverHex();
        setCellDistance();
    }
    void setCellDistance()
    {
        float tempD = Vector2.Distance(
            BattleManager.Instance.hexgrid.GetCellInCoord(Vector3Int.zero).transform.position,
            BattleManager.Instance.hexgrid.GetCellInCoord(Vector3Int.right).transform.position
            );
        BattleManager.Instance.hexgrid.CellDistance = tempD;
    }
    void AutoGenFullCoverHex()
    {
        
        float hexWidth = HexCellMeshGenerator.GetHexagonWidth();
        float innerRadius = HexCellMeshGenerator.GetInnerRadius();
        float outerRadius = HexCellMeshGenerator.OUTER_RADIUS;
        Vector3 topLeftWorldLoc = GetLeftTopCorner(this.GetComponent<MeshFilter>().mesh, this.transform);
    
        // Convert world position to local position
        Vector3 topLeftLocalLoc = transform.InverseTransformPoint(topLeftWorldLoc);
        Vector3 startLocalLoc = new Vector3(topLeftLocalLoc.x + innerRadius, 0.1f, topLeftLocalLoc.z - innerRadius);


        Vector3 rightBotLocalLoc = transform.InverseTransformPoint(GetRightBottomCorner(this.GetComponent<MeshFilter>().mesh, this.transform));
        
        int c = 0;
        int r = 0;
        float xStartPos = startLocalLoc.x;
        float zStartPos =  (startLocalLoc.z - c * (innerRadius + outerRadius/2));
        do
        {
            if (c % 2 == 0) xStartPos = startLocalLoc.x;
            if (c % 2 == 1) xStartPos = startLocalLoc.x + hexWidth/2;
            r = 0;
            do
            {
                HexCellComponent newCell = Instantiate(hexCellPrefab, transform);
                newCell.transform.localPosition = new Vector3(xStartPos, 0.1f,zStartPos);
                newCell.gameObject.name = "HexCell ( " + r + " , " + c + " )";
                // Check for collision and destroy if necessary
                if (newCell.GetComponent<HexCellMeshGenerator>().CheckForCollisionAtCurrentPosition())
                {
                    // Debug.Log($"Hex at position {(r, c)} overlaps with another object. Destroying.");

                    HexCell cellData = new HexCell("HexCell ( " + r + " , " + c + " )", new Vector3Int(r, 0, c), CellType.Invalid, newCell);
                    newCell.Initialize(cellData);
                }
                else
                {
                    HexCell cellData = new HexCell("HexCell ( " + r + " , " + c + " )", new Vector3Int(r, 0, c), CellType.Empty, newCell);
                    newCell.Initialize(cellData);
                }
                
                BattleManager.Instance.hexgrid.AddCell(newCell);
                xStartPos += hexWidth;
                r += 1;
            } while (xStartPos < rightBotLocalLoc.x - innerRadius);
            
            c += 1;
            zStartPos = (startLocalLoc.z - c * (innerRadius + outerRadius / 2)) - c * zOffset;
        } while (zStartPos > rightBotLocalLoc.z );

        BattleManager.Instance.hexgrid.Height = c;
        BattleManager.Instance.hexgrid.Width = r;
    }
    
    Vector3 GetLeftTopCorner(Mesh mesh, Transform meshTransform)
    {
        Vector3[] vertices = mesh.vertices;
        
        // Transform vertices to world space
        Vector3[] worldVertices = vertices.Select(v => meshTransform.TransformPoint(v)).ToArray();

        // Find the vertex with minimum x and maximum z
        Vector3 leftTopCorner = worldVertices[0];
        foreach (Vector3 vertex in worldVertices)
        {
            if (vertex.x < leftTopCorner.x || (vertex.x == leftTopCorner.x && vertex.z > leftTopCorner.z))
            {
                leftTopCorner = vertex;
            }
        }
        return leftTopCorner;
    }
    Vector3 GetRightBottomCorner(Mesh mesh, Transform meshTransform)
    {
        Vector3[] vertices = mesh.vertices;
    
        // Transform vertices to world space
        Vector3[] worldVertices = vertices.Select(v => meshTransform.TransformPoint(v)).ToArray();

        // Find the vertex with maximum x and minimum z
        Vector3 rightBottomCorner = worldVertices[0];
        foreach (Vector3 vertex in worldVertices)
        {
            if (vertex.x > rightBottomCorner.x || (vertex.x == rightBottomCorner.x && vertex.z < rightBottomCorner.z))
            {
                rightBottomCorner = vertex;
            }
        }
        return rightBottomCorner;
    }
    
    private void Awake()
    {
        
    }
    
    
}