using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class GamePauseManager : MonoBehaviour 
{
[Header("Pause Settings")]
[SerializeField] private KeyCode pauseKey = KeyCode.Escape;
[SerializeField] private GameObject pauseMenuUI;
[SerializeField] private bool pauseCursorVisible = true;
[SerializeField] private CursorLockMode pauseCursorMode = CursorLockMode.None;

[SerializeField] private GameObject MenuButton;

private bool originalCursorVisible;
private CursorLockMode originalCursorMode;

// Track pause state
private bool isPaused = false;

private void Start()
{
    if (pauseMenuUI != null)
    {
        pauseMenuUI.SetActive(false);
    }
    
    // Store original cursor state
    originalCursorVisible = Cursor.visible;
    originalCursorMode = Cursor.lockState;
    
    // Ensure we have an EventSystem in the scene
    if (FindObjectOfType<EventSystem>() == null)
    {
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
        Debug.Log("Added EventSystem to scene");
    }
}

private void Update()
{
    if (Input.GetKeyDown(pauseKey))
    {
        TogglePause();
    }
}

public void TogglePause()
{
    if (isPaused)
    {
        ResumeGame();
    }
    else
    {
        PauseGame();
    }
}

public void PauseGame()
{
    MenuButton.SetActive(false);

    Time.timeScale = 0f;
    
    if (pauseMenuUI != null)
    {
        pauseMenuUI.SetActive(true);
    }

    Cursor.visible = pauseCursorVisible;
    Cursor.lockState = pauseCursorMode;
    
    isPaused = true;
    
    
    Debug.Log("Game Paused");
}

public void ResumeGame()
{
    MenuButton.SetActive(true);
    Time.timeScale = 1f;
    
    if (pauseMenuUI != null)
    {
        pauseMenuUI.SetActive(false);
    }
    
    Cursor.visible = originalCursorVisible;
    Cursor.lockState = originalCursorMode;
    
    isPaused = false;
    
    
    Debug.Log("Game Resumed");
}

public void OnResumeButtonClicked()
{
    ResumeGame();
}

public void OnMainMenuButtonClicked()
{
    Time.timeScale = 1f; 

    UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
}

public void OnQuitButtonClicked()
{
    Time.timeScale = 1f;
    
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
}
}