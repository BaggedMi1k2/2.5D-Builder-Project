using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // Teleporter settings
    public float teleportCooldown = 2f;
    public string playerTag = "Player";

    public GridPlacement gridPlacement;

    private static List<Teleporter> activeTeleporters = new List<Teleporter>();
    private static bool isTeleporting = false;


    void Awake()
    {
        isTeleporting = false;
    }

    void Start()
    {
        //check if placed
        if (CompareTag("PlacedObject"))
        {
            RegisterTeleporter();
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    void RegisterTeleporter()
    {
        activeTeleporters.Add(this);

        // If there are more than 2 teleporters remove the old one
        if (activeTeleporters.Count > 2)
        {
            Teleporter oldest = activeTeleporters[0];
            if (oldest != null)
            {
                if (oldest.gridPlacement != null)
                {
                    Vector3 snappedPos = new Vector3(
                        Mathf.Round(oldest.transform.position.x / oldest.gridPlacement.gridSize) * oldest.gridPlacement.gridSize,
                        Mathf.Round(oldest.transform.position.y / oldest.gridPlacement.gridSize) * oldest.gridPlacement.gridSize,
                        Mathf.Round(oldest.transform.position.z / oldest.gridPlacement.gridSize) * oldest.gridPlacement.gridSize
                    );
                    oldest.gridPlacement.occupiedPositions.Remove(snappedPos);
                }
                Destroy(oldest.gameObject);
            }
            activeTeleporters.RemoveAt(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CompareTag("PlacedObject"))
            return;

        if (isTeleporting)
            return;

        if (other.CompareTag(playerTag))
        {
            StartCoroutine(TeleportPlayer(other.gameObject));
        }
    }

    IEnumerator TeleportPlayer(GameObject player)
    {
        if (isTeleporting)
            yield break;

        if (activeTeleporters.Count < 2)
            yield break;

        Teleporter target = activeTeleporters.Find(t => t != this);
        if (target == null)
            yield break;

        isTeleporting = true;

        // Teleport player
        player.transform.position = target.transform.position + Vector3.up * 1f;
        SoundManager.Instance.PlaySound2D("Teleport");
        //cooldown
        yield return new WaitForSeconds(teleportCooldown);
        isTeleporting = false;
    }
}
