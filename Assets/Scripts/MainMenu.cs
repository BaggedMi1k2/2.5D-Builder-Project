using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        MusicManager.Instance.PlayMusic("Menu");
    }

    public void LoadLevelByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        SoundManager.Instance.PlaySound2D("Select");
        MusicManager.Instance.PlayMusic("Game");
    }

    public void QuitGame()
    {
        SoundManager.Instance.PlaySound2D("Select");
        Debug.Log("Quit Game triggered.");
        Application.Quit();
    }
}
