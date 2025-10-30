using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject[] objectsToToggle;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.StopTimer();

            if (objectsToToggle != null && objectsToToggle.Length > 0)
            {
                foreach (GameObject obj in objectsToToggle)
                {
                    if (obj != null)
                    {
                        obj.SetActive(!obj.activeSelf);
                    }
                }
            }
        }
        Time.timeScale = 0f;
    }

    
}