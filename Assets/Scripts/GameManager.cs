using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class KeyToggle
{
    public KeyCode key;        // The key to press
    public GameObject target;  // The object to toggle
}

[System.Serializable]
public class KeyScene
{
    public KeyCode key;        // The key to press
    public string sceneName;   // The scene to load (must be in Build Settings)
}

public class GameManager : MonoBehaviour
{
    public KeyToggle[] toggles;
    public KeyScene[] sceneBindings;
    public KeyCode resetKey = KeyCode.P;

    public TMP_Text[] timerText;
    private float timer = 0f;
    private bool timerRunning = true;

    public GameObject objectToToggle;

    private void Awake()
    {
        Time.timeScale = 1f;
    }
    private void Update()
    {
        // Timer logic
        if (timerRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerDisplay();
        }


        // Toggle objects
        foreach (var toggle in toggles)
        {
            if (Input.GetKeyDown(toggle.key) && toggle.target != null)
            {
                bool newState = !toggle.target.activeSelf;
                toggle.target.SetActive(newState);

                //handle GridPlacement ghost visibility
                GridPlacement gridPlacement = toggle.target.GetComponent<GridPlacement>();
                if (gridPlacement != null)
                {
                    if (newState)
                        gridPlacement.ShowGhost();
                    else
                        gridPlacement.HideGhost();
                }
            }
        }

        foreach (var binding in sceneBindings)
        {
            if (Input.GetKeyDown(binding.key) && !string.IsNullOrEmpty(binding.sceneName))
            {
                SceneManager.LoadScene(binding.sceneName);
            }
        }

        // Restart current scene
        if (Input.GetKeyDown(resetKey))
        {
            RestartGame();
        }

    }


    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        string timeString = $"{minutes:00}:{seconds:00}";

        if (timerText != null)
        {
            foreach (var text in timerText)
            {
                if (text != null)
                    text.text = timeString;
            }
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public static void RestartGame()
    {
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadNextLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int nextSceneIndex = currentScene.buildIndex + 1;

        // Check if next scene exists
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels in build settings. Restarting first level.");
            SceneManager.LoadScene(0); // loop back to main menu if no nother levels
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0); // loop back to main menu if no nother levels
        
    }

    public void pause()
    {
        if (objectToToggle != null)
        {
            bool isActive = objectToToggle.activeSelf;
            objectToToggle.SetActive(!isActive);

            // Pause or unpause the game
            if (Time.timeScale == 1f)
            {
                Time.timeScale = 0f; // Pause
            }
            else
            {
                Time.timeScale = 1f; // Unpause
            }
        }

    }
}