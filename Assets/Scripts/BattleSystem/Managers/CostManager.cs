using System;
using UnityEngine;
using System.Collections;

public class CostManager : Singleton<CostManager>
{
	public int MaxCost;

	[Tooltip("Seconds")] public float CDPerCost { get; private set; } = 1;
	public int availCost { get; private set; }
	private static bool s_onCostBeCal = true;

	public Action OnCostUpdated;
	
	private void Start()
	{
		availCost = 0;
		//StartCoroutine(_CostCalCoroutine());
	}
	
	private IEnumerator _CostCalCoroutine()
	{
		while (s_onCostBeCal)
		{
			yield return new WaitForSeconds(CDPerCost);
			if (availCost < MaxCost)
			{
			 	availCost += 1;
			    OnCostUpdated.Invoke();
			 	//Debug.Log(availCost);
			}
		}

		yield return null;
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	public void SetCdPerCost(float newCD)
	{
		CDPerCost = newCD;
	}
	public void SetAvailCost(int _availCost)
	{
		availCost = _availCost;
	}

	public void AddAvailCost(int addAmount)
	{
		Mathf.Clamp(availCost , availCost += addAmount,5);
		OnCostUpdated.Invoke();
	}
	
	public bool MinusAvailCost(int minusAmount)
	{
		int newAvailCost = availCost - minusAmount;
		if (newAvailCost < 0) return false;
    
		availCost = newAvailCost;
		OnCostUpdated.Invoke();
		return true;
	}
}