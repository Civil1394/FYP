using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
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
			foreach (var cell in BattleManager.Instance.hexgrid.playerSixDirCellsSet)
			{
				if (cell != null)
				{
					Gizmos.color = Color.green;
					Gizmos.DrawSphere(cell.transform.position + Vector3.up * 0.5f, 0.5f); 
				}
			}
		}
		
	}

	public void DebugDrawPlayerDirCell()
	{
		
		IsDebugDrawPlayerDirCell = !IsDebugDrawPlayerDirCell;
		
	}
}

