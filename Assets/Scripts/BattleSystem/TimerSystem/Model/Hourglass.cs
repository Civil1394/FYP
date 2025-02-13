using UnityEngine;
using System.Collections;
[System.Serializable]
public class Hourglass
{
	public int Sand;
	public TimeType TimeType;
	public string Id; // unique identifier 
	
	public Hourglass(int sand , TimeType timeTimeType)
	{
		this.Sand = sand;
		this.TimeType = timeTimeType;
		Id = Helpers.GetUniqueID(this);
	}
}

public enum TimeType
{
	Boost,
	Slow,
	None
}