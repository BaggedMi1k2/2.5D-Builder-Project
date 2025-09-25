using UnityEngine;

[System.Serializable]
public class KeyToggle
{
    public KeyCode key;        // The key to press
    public GameObject target;  // The object to toggle
}

public class GameManager : MonoBehaviour
{
    public KeyToggle[] toggles;
    private void Update()
    {
        foreach (var toggle in toggles)
        {
            if (Input.GetKeyDown(toggle.key) && toggle.target != null)
            {
                toggle.target.SetActive(!toggle.target.activeSelf);
            }
        }
    }
}