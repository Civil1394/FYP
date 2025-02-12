using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameManager : Singleton<GameManager>
{
	
	public List<Hourglass> hourglasses = new List<Hourglass>();

	private void Start()
	{
		for (int i = 0; i < 10; i++)
		{
			hourglasses.Add(new Hourglass(Random.Range(1,10),TimeManipulationType.Boost));
		}
	}
	
	private bool IsDebugDrawPlayerDirCell = false;
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
}

