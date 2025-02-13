using UnityEngine;
using System.Collections;

public class HourglassFactory : MonoBehaviour 
{
	public GameObject HourglassPrefab;
	public Hourglass CreateHourglass(int sand , TimeType timeType)
	{
		Hourglass hourglass = new Hourglass(sand, timeType);
		return hourglass;
	}
}