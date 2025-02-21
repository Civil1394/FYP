using UnityEngine;
using System.Collections;

public class ChestSpawner : MonoBehaviour
{
	public enum ChestType
	{
		Wood,
		Steel,
		Gold,
		Legend
	}
	public float woodChestRate;
	public float steelChestRate;
	public float goldChestRate;
	public float legendaryChestRate;
	public int maxChestCount;
	public Vector2Int[] staticLegendaryChestLocation;

	public ChestType[,] GetChestHeatMap(int width, int height)
	{
		float[,] rawChestMap = NoiseSystem.GenerateNoiseMap(width, height, 1);
		
		return null;
	}
}