using UnityEngine;
using System.Collections;

public class HexMapGenerator : MonoBehaviour
{
	public HexGrid hexGrid;
	public Collider surfaceCollider;

	void Start()
	{
		// hexGrid.HexSize = 1f; // Set your desired hex size
		// hexGrid.FitToSurface(surfaceCollider);
	}
}