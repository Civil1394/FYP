using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public static class NoiseSystem 
{
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

	public static List<Vector2Int> BlobDetection(float[,] noiseMap, float valueThreshold, int distThreshold)
	{
		//implement the blob detection algorithm
		//return the center of each blob
		//center should be calculated with the weight of the pixel intensity
		List<Vector2Int> blobList = new List<Vector2Int>();
		List<Vector2Int> blobCenterList = new List<Vector2Int>();
		Debug.Log(noiseMap.Length);
		for (int i = 0; i < noiseMap.GetLength(0); i++)
		{
			for (int j = 0; j < noiseMap.GetLength(1); j++)
			{
				Debug.Log(noiseMap[i, j]);
				if (noiseMap[i, j] > valueThreshold)
				{
					blobList.Add(new Vector2Int(i, j));
				}
			}
		}
		blobCenterList.Add(blobList[0]);
		foreach (var b in blobList)
		{
			foreach (var bc in blobCenterList)
			{
				var tempDis = Vector2Int.Distance(b, bc);
				if (tempDis > distThreshold)
				{
					blobCenterList.Add(b);
				}
				else
				{
					bc.Set((b.x + bc.x) / 2,(b.y + bc.y) / 2);
				}
			}
		}
		Debug.Log(blobCenterList.Count);
		return blobCenterList;
	}
}