using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
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
}

