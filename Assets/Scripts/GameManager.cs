using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class KeyToggle
{
    public KeyCode key;        // The key to press
    public GameObject target;  // The object to toggle
}

public class GameManager : MonoBehaviour
{
    public KeyToggle[] toggles;
    public KeyCode resetKey = KeyCode.P;

    public TMP_Text timerText;
    private float timer = 0f;
    private bool timerRunning = true;


    private void Update()
    {

        if (timerRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerDisplay();
        }

        foreach (var toggle in toggles)
        {
            if (Input.GetKeyDown(toggle.key) && toggle.target != null)
            {
                toggle.target.SetActive(!toggle.target.activeSelf);
            }
        }

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
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

}