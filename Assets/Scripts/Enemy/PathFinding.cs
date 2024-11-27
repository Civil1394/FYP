using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Node
{
    public HexCell HexCell { get; set; }
    public float Heuristic { get; set; }
    public float Cost { get; set; }
    public Node Parent { get; set; }
    public float TotalCost { get { return Cost + Heuristic; } }

    public Node(HexCell hexCell, float cost, float heuristic, Node parent = null)
    {
        HexCell = hexCell;
        Cost = cost;
        Heuristic = heuristic;
        Parent = parent;
    }
}


public class NodeComparer : IComparer<Node>
{
    public int Compare(Node x, Node y)
    {
        return x.TotalCost.CompareTo(y.TotalCost);
    }
}

public class PathFinding
{
    public PathFinding() { }

    public async Task<List<HexCell>> FindPathAsync(HexCellComponent start, HexCellComponent end)
    {
        return await Task.Run(() => FindPath(start, end));
    }

    public List<HexCell> FindPath(HexCellComponent start, HexCellComponent end)
    {
        bool stopOneCellEarlier = end.CellData.CellType != CellType.Empty;
        SortedSet<Node> openList = new SortedSet<Node>(new NodeComparer());
        HashSet<HexCell> isVisited = new HashSet<HexCell>();
        float h = CalculateHValue(start.CellData, end);
        openList.Add(new Node(start.CellData, 0, h));
        if (end.CellData.CellType != CellType.Empty)
        {
            stopOneCellEarlier = true;
        }
        while (openList.Count > 0)
        {
            Node currentNode = openList.Min;
            isVisited.Add(currentNode.HexCell);
            openList.Remove(currentNode);

            if ((stopOneCellEarlier && currentNode.HexCell.GetAllNeighbor().Contains(end.CellData)) || currentNode.HexCell == end.CellData)
            {
                return ReconstructPath(currentNode);
            }

            foreach (HexCell nextCell in currentNode.HexCell.GetAllNeighbor())
            {
                if (nextCell == null || isVisited.Contains(nextCell) || nextCell.CellType != CellType.Empty || EnemyManager.Instance.IsCellReserved(nextCell)) continue;
                h = CalculateHValue(nextCell, end);
                openList.Add(new Node(nextCell, currentNode.Cost + 1, h, currentNode));
            }
        }

        return null; // No path found
    }

    private List<HexCell> ReconstructPath(Node currentNode)
    {
        List<HexCell> path = new List<HexCell>();
        while (currentNode != null)
        {
            path.Add(currentNode.HexCell);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }

    public float CalculateHValue(HexCell current, HexCellComponent end)
    {
        return Vector3.Distance(current.Coordinates, end.CellData.Coordinates);
    }
}