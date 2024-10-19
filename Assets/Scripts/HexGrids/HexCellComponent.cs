using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class HexCellComponent : MonoBehaviour
{

	public HexCell CellData;
	private MeshRenderer meshRenderer;
	[SerializeField] private Material emptyCellMat;
	[SerializeField]private Material validCellMat;
	[SerializeField] private Color validColor;
	[SerializeField] private Color invalidColor;
	public void Initialize(HexCell hexCell)
	{
		CellData = hexCell;
		meshRenderer = this.GetComponent<MeshRenderer>();
		emptyCellMat= new Material(meshRenderer.material);
		validCellMat = new Material(meshRenderer.material);
		
		if(hexCell.CellType == CellType.Empty)validCellMat.color = validColor;
		if (hexCell.CellType == CellType.Invalid)
		{
			emptyCellMat.color = invalidColor;
			validCellMat.color = invalidColor;
			meshRenderer.material = validCellMat;
		}
	}

	public void DebugTest()
	{
		this.gameObject.transform.localPosition += Vector3.up * 10;
		foreach (var c in CellData.Neighbors)
		{
			if(c!=null)
				Debug.Log($"{c.Coordinates}");
		}
	}

	public void ChangeCellColor(bool pointEnter)
	{
		if (pointEnter)
			meshRenderer.material = validCellMat;
		else
			meshRenderer.material = emptyCellMat;
	}
}

[System.Serializable]
public class HexCell
{
	public string ID;
	public Vector3Int Coordinates;
	public CellType CellType;
	
	[NonSerialized] public HexCell[] Neighbors = new HexCell[6];
	public HexCell (string ID,Vector3Int coordinates , CellType cellType)
	{
		this.ID = ID;
		this.Coordinates = coordinates;
		this.CellType = cellType;
	}
	
	
	// Helper method to get a neighbor in a specific direction
	public HexCell GetNeighbor(HexDirection direction)
	{
		return Neighbors[(int)direction];
	}
    
	// Helper method to set a neighbor in a specific direction
	public void SetNeighbor(HexDirection direction, HexCell cell)
	{
		Neighbors[(int)direction] = cell;
		cell.Neighbors[(int)direction.Opposite()] = this;
	}
}