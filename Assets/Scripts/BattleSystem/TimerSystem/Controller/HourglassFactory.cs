using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

public class HourglassFactory : Singleton<HourglassFactory>
{
	public GameObject HourglassPrefab;

	public List<Hourglass> CreateHourglasses(int num, bool isRandom, int? sand, TimeType? timeType, int?MaxThreshold)
	{
		// If not random, ensure we have valid parameters.
		if (!isRandom && (!sand.HasValue || !timeType.HasValue))
			throw new ArgumentException("Sand and TimeType must be provided when isRandom is false.");

		var hourglasses = new List<Hourglass>(num);
		int timeTypeCount = Enum.GetValues(typeof(TimeType)).Length;

		for (int i = 0; i < num; i++)
		{
			int actualSand = isRandom ? Random.Range(1, 10) : sand.Value;
			TimeType actualTimeType = isRandom 
				? (TimeType)Random.Range(0, timeTypeCount) 
				: timeType.Value;
			int actualMaxThreshold = isRandom ? Random.Range(actualSand, actualSand + 10) : MaxThreshold.Value;
			hourglasses.Add(CreateSingleHourglass(actualSand + 1, actualTimeType ,actualMaxThreshold));
		}
		return hourglasses;
	}
	
	public Hourglass CreateSingleHourglass(int sand , TimeType timeType,int MaxThreshold)
	{
		Hourglass hourglass = new Hourglass(sand, timeType, MaxThreshold,false);
		return hourglass;
	}
}