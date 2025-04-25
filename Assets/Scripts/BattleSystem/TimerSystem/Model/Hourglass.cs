using UnityEngine;
using System.Collections;
[System.Serializable]
public class Hourglass
{
	public float Sand;
	public TimeType TimeType;
	public string Id; // unique identifier 
	
	public float MinThreshold = 0f;
	public float MaxThreshold = 3f;
	public bool IsOccupied;
	public Hourglass(float sand , TimeType timeTimeType,float MaxThreshold,bool IsOccupied)
	{
		this.Sand = sand;
		this.TimeType = timeTimeType;
		this.MaxThreshold = MaxThreshold;
		this.IsOccupied = IsOccupied;
		
		Id = Helpers.GetUniqueID(this);
	}
}

public enum TimeType
{
	Boost = 0,
	Slow = 1,
	None = 2
}