using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public static class NoiseSystem 
{
	public class Blob
	{
		public Vector2Int position;
		public float intensity;
		public Blob parent;
		public Blob(Vector2Int position, float intensity)
		{
			this.position = position;
			this.intensity = intensity;
		}
	}
	
	public static float[,] GenerateNoiseMap(int width, int height, float scale)
	{
		float[,] noiseMap = new float[width, height];
		if (scale <= 0)
		{
			scale = 0.0001f;
		}
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float sampleX = x/scale;
				float sampleY = y/scale;
				float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
				noiseMap[x, y] = perlinValue;
			}
		}
		return noiseMap;
	}

	private static IEnumerable<Vector2Int> Threshold(float[,] noiseMap, float threshold)
	{
		for (int i = 0; i < noiseMap.GetLength(0); i++)
		{
			for (int j = 0; j < noiseMap.GetLength(1); j++)
			{
				Debug.Log(noiseMap[i, j]);
				if (noiseMap[i, j] >= threshold)
				{
					yield return new Vector2Int(i,j);
				}
			}
		}
	}

	public static List<Vector2Int> GetCenterPosition(float[,] noiseMap, int numOfPoints, float minDist)
	{
		List<Vector2Int> pointList = new List<Vector2Int>();
		float threshold = 1;
		while (pointList.Count < numOfPoints)
		{
			pointList.AddRange(Threshold(noiseMap, threshold));
			threshold -= 0.1f;
		}
		return pointList;
	}
	// public static List<Vector2Int> BlobDetection(float[,] noiseMap, float valueThreshold, int distThreshold)
	// {
	// 	//implement the blob detection algorithm
	// 	//return the center of each blob
	// 	//center should be calculated with the weight of the pixel intensity
	// 	List<Blob> blobList = new List<Blob>();
	// 	List<Blob> blobCenterList = new List<Blob>();
	// 	for (int i = 0; i < noiseMap.GetLength(0); i++)
	// 	{
	// 		for (int j = 0; j < noiseMap.GetLength(1); j++)
	// 		{
	// 			Debug.Log(noiseMap[i, j]);
	// 			if (noiseMap[i, j] > valueThreshold)
	// 			{
	// 				blobList.Add(new Blob(new Vector2Int(i, j), noiseMap[i, j]));
	// 			}
	// 		}
	// 	}
	// 	blobCenterList.Add(blobList[0]);
	// 	foreach (var b in blobList)
	// 	{
	// 		foreach (var bc in blobCenterList)
	// 		{
	// 			var tempDis = Vector2Int.Distance(b.position, bc.position);
	// 			if (tempDis > distThreshold)
	// 			{
	// 				blobCenterList.Add(b);
	// 			}
	// 			else
	// 			{
	// 				bc.position = (b.position*b.intensity+bc.position*bc.intensity)/bc.intensity+b.intensity;;
	// 				bc.intensity+=distThreshold;
	// 			}
	// 		}
	// 	}
	// 	Debug.Log(blobCenterList.Count);
	// 	return blobCenterList;
	// }
}