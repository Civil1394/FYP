using System;
using UnityEngine;
using TMPro;
public class HexCellComponent : MonoBehaviour
{
    public HexCell CellData;
    private MeshRenderer meshRenderer;
    [SerializeField] private Material sharedCellMat;
    [SerializeField] private Color emptyColor;
    [SerializeField] private Color invalidColor;
    [SerializeField] private Color validMoveRangeColor;
    [SerializeField] private Color validAttackRangeColor;
    [SerializeField] private Color objectStandingColor;

    
    [Header("Debug")]
    public TextMeshProUGUI DebugCoord;
    private Material customCellMat;

    public void Initialize(HexCell hexCell)
    {
        CellData = hexCell;
        meshRenderer = this.GetComponent<MeshRenderer>();
        customCellMat = new Material(meshRenderer.material);
        meshRenderer.material = customCellMat;
        UpdateMaterialColor();
        CellData.OnCellTypeChanged += UpdateMaterialColor;
        DebugCoord.text = CellData.Coordinates.ToString();
        DebugManager.Instance.CellsCoordGUI.Add(DebugCoord);
    }

    private void OnDestroy()
    {
        if (CellData != null)
        {
            CellData.OnCellTypeChanged -= UpdateMaterialColor;
        }
    }

    private void UpdateMaterialColor()
    {
        switch (CellData.CellType)
        {
            case CellType.Empty:
                customCellMat.color = emptyColor;
                break;
            case CellType.Invalid:
            case CellType.Enemy:
                customCellMat.color = invalidColor;
                break;
        }

        switch (CellData.CellGuiType)
        {
            case CellGuiType.ValidMoveCell:
                customCellMat.color = validMoveRangeColor;
                break;
            case CellGuiType.ValidAttackCell:
                customCellMat.color = validAttackRangeColor;
                break;
            case CellGuiType.Chest:
                customCellMat.color = Color.green;
                break;
            case CellGuiType.Empty:
                UpdateCellTypeColor();
                break;
        }

        meshRenderer.material = customCellMat;
    }

    private void UpdateCellTypeColor()
    {
        switch (CellData.CellType)
        {
            case CellType.Empty:
                customCellMat.color = emptyColor;
                break;
            case CellType.Player:
                customCellMat.color = objectStandingColor;
                break;
            case CellType.Invalid:
            case CellType.Enemy:
                customCellMat.color = invalidColor;
                break;
        }
    }

    public Vector3 CalPosForAction()
    {
        return new Vector3(transform.position.x, 0.9f, transform.position.z);
    }

    public void DebugTest()
    {
        this.gameObject.transform.localPosition += Vector3.up * 10;
        foreach (var c in CellData.Neighbors)
        {
            if (c != null)
                Debug.Log($"{c.Coordinates}");
        }
    }

    public void ChangeCellColor(bool pointEnter)
    {
        if (pointEnter)
            meshRenderer.material = customCellMat;
        else
            meshRenderer.material = sharedCellMat;
    }
}

[System.Serializable]
public class HexCell
{
    public string ID;
    public Vector3Int Coordinates;
    [SerializeField] private CellType cellType;
    [SerializeField] private CellGuiType cellGuiType;

    public CellType CellType
    {
        get => cellType;
        set
        {
            if (cellType != value)
            {
                cellType = value;
                OnCellTypeChanged?.Invoke();
            }
        }
    }

    public CellGuiType CellGuiType
    {
        get => cellGuiType;
        set
        {
            if (cellGuiType != value)
            {
                cellGuiType = value;
                OnCellTypeChanged?.Invoke();
            }
        }
    }

    public Action OnCellTypeChanged;

    [System.NonSerialized] public HexCell[] Neighbors = new HexCell[6];
    [System.NonSerialized] public readonly HexCellComponent ParentComponent;
    [System.NonSerialized] public GameObject StandingGameObject;
    public HexCell(string ID, Vector3Int coordinates, CellType cellType, HexCellComponent parentComponent)
    {
        this.ID = ID;
        this.Coordinates = coordinates;
        this.CellType = cellType;
        this.CellGuiType = CellGuiType.Empty;
        ParentComponent = parentComponent;
    }

    public void ClearCell()
    {
        CellType = CellType.Empty;
        StandingGameObject = null;
    }

    public void SetCell(GameObject standingGO, CellType newCellType)
    {
        StandingGameObject = standingGO;
        CellType = newCellType;
    }
    [Obsolete("SetType is deprecated, please use SetCell or ClearCell instead.", true)]
    public void SetType(CellType newCellType)
    {
        CellType = newCellType;
    }

    public void SetGuiType(CellGuiType newCellGuiType)
    {
        CellGuiType = newCellGuiType;
    }

    public void ResetCuiType()
    {
        //show have a function that reset the cell gui type?
    }
    public HexCell GetNeighbor(HexDirection direction)
    {
        return Neighbors[(int)direction];
    }

    public HexCell[] GetAllNeighbor()
    {
        return Neighbors;
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        Neighbors[(int)direction] = cell;
        cell.Neighbors[(int)direction.Opposite()] = this;
    }
}