using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HourglassesUIContainer : Singleton<HourglassesUIContainer>
{
	[SerializeField] private GameObject hourglassUIPrefab;
	public int SlotsAmount;
	public List<HourglassUIProduct> HourglassSlots = new List<HourglassUIProduct>();

	private void Awake()
	{
		for (int i = 0; i < SlotsAmount; i++)
		{
			GenerateEmptyHourglassProduct();
		}
	}

	private void GenerateEmptyHourglassProduct()
	{
		GameObject emptyHourglass = Instantiate(this.hourglassUIPrefab,this.transform);
		HourglassSlots.Add(emptyHourglass.GetComponent<HourglassUIProduct>());
	}
	
		
	public void InitHourglassProducts(Hourglass[] hourglasses)
	{
		int index = 0;
		foreach (var slot in HourglassSlots)
		{
			if (!slot.hourglass.IsOccupied)
			{
				slot.Init(hourglasses[index]);
				index++;
			}
		}
	}

	public void InitHourglassProducts(Hourglass hourglass)
	{
		foreach (var slot in HourglassSlots)
		{
			if (!slot.hourglass.IsOccupied)
			{
				slot.Init(hourglass);
				break; 
			}
		}
	}

	
	
}