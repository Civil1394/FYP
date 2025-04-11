using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using Unity.VisualScripting;

public class DebugManager : Singleton<DebugManager>
{
	
	public List<Hourglass> hourglasses = new List<Hourglass>();
	public List<TextMeshProUGUI> CellsCoordGUI = new List<TextMeshProUGUI>();
	private bool IsDebugDrawPlayerDirCell = false;
	
	private void Start()
	{
		for (int i = 0; i < 10; i++)
		{
			//hourglasses.Add(new Hourglass(Random.Range(1,10),TimeType.Boost));
		}
	}

	private void Update()
	{
		//DebugCastUse();
	}

	public void Quit()
	{
		// For testing in the Unity editor
		#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
		#else
		        // For a built game
		        Application.Quit();
		#endif
	}

	private void OnDrawGizmos()
	{
		if(IsDebugDrawPlayerDirCell)
		{
			foreach (var cell in BattleManager.Instance.hexgrid.PlayerSixDirCellsSet)
			{
				if (cell.Key != null)
				{
					Gizmos.color = Color.green;
					Gizmos.DrawSphere(cell.Key.transform.position + Vector3.up * 0.5f, 0.5f); 
				}
			}
		}
	}

	public void DebugDrawPlayerDirCell()
	{
		
		IsDebugDrawPlayerDirCell = !IsDebugDrawPlayerDirCell;
		
	}

	public void DebugCellsCoordGUI()
	{
		foreach (var coord in CellsCoordGUI)
		{
			coord.gameObject.transform.parent.gameObject.SetActive(!coord.gameObject.transform.parent.gameObject.activeSelf);
		}
	}

	public void PrintPlayerDirCell()
	{
		if(IsDebugDrawPlayerDirCell)
		{
			foreach (var pair in BattleManager.Instance.hexgrid.PlayerSixDirCellsSet)
			{
				HexCellComponent cell = pair.Key;
				int value = pair.Value;
            
				// Print to console
				Debug.Log($"Cell: {cell.name} - Direction: {value}");
				
			}
		}
	}
	// public void DebugCastUse()
	// {
	// 	if (Input.GetKeyDown(KeyCode.LeftShift)&&Input.GetKeyDown(KeyCode.D))
	// 	{
	// 		var e = EquippedAbilityManager.GetEquippedAbilityData((int)HexDirection.E);
	// 		e.TriggerAbility(CasterType.Player,HexDirection.E,BattleManager.Instance.PlayerCell,BattleManager.Instance.gameObject,TimeType.Boost);
	// 	}
	// }
}

