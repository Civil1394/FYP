using UnityEngine;
using System.Collections.Generic;

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

	public static List<Vector2Int> BlobDetection(float[,] noiseMap, int thresold, int blobCount)
	{
		//implement the blob detection algorithm
		//return the center of each blob
		//center should be calculated with the weight of the pixel intensity
		return null;
	}
}