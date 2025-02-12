using UnityEngine;
using System.Collections;
[System.Serializable]
public class Hourglass
{
	public int sand;
	public TimeManipulationType manipulationType;
	public string Id; // unique identifier 
	
	public Hourglass(int sand , TimeManipulationType timeManipulationType)
	{
		this.sand = sand;
		this.manipulationType = timeManipulationType;
		Id = Helpers.GetUniqueID(this);
	}
}

public enum TimeManipulationType
{
	Boost,
	Slow,
	None
}