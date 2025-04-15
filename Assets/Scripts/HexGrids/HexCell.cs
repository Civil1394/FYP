using System;
using UnityEngine;

[Serializable]
public class HexCell
{
    public string ID;
    public Vector3Int Coordinates;
    [SerializeField] private CellType cellType;
    [SerializeField] private CellActionType cellActionType;

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

    public CellActionType CellActionType
    {
        get => cellActionType;
        set
        {
            if (cellActionType != value)
            {
                cellActionType = value;
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
        this.CellActionType = CellActionType.Empty;
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

    public void SetGuiType(CellActionType newCellActionType)
    {
        CellActionType = newCellActionType;
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