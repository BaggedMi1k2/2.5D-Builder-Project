using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

    public TMP_Text timerText;
    private float timer = 0f;
    private bool timerRunning = true;


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
                toggle.target.SetActive(!toggle.target.activeSelf);
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
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}