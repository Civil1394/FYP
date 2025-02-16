using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HourglassInventory:Singleton<HourglassInventory>
{
	public List<Hourglass> hourglassesList = new List<Hourglass>();

	public Hourglass GetRandomHourglassFromInventory()
	{
		var unoccupiedHourglasses = hourglassesList.Where(h => !h.IsOccupied).ToList();
		return unoccupiedHourglasses[Random.Range(0, unoccupiedHourglasses.Count)];
	}
}