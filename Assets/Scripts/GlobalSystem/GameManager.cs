using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace FYP
{
	public class GameManager : MonoBehaviour
	{
		public void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}

		public void QuitGame()
		{
			#if UNITY_EDITOR
						UnityEditor.EditorApplication.isPlaying = false;
			#else
					        // For a built game
					        Application.Quit();
			#endif
		}
	}
}
