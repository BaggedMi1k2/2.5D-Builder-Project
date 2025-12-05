using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    //Bomb Settings
    public float countdownTime = 5f;
    public float explosionRadius = 5f;
    public GameObject explosionPrefab; // Explosion FX
    public string destructibleTag = "Destructable";

    //UI
    public TMP_Text countdownText;

    private float timer;
    public GridPlacement gridPlacement;

    void Start()
    {
        if (CompareTag("PlacedObject"))
        {
            timer = countdownTime;
            if (countdownText != null)
                countdownText.text = timer.ToString("F0");
            StartCoroutine(Countdown());
        }
    }

    IEnumerator Countdown()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer -= 1f;

            if (countdownText != null)
                countdownText.text = timer.ToString("F0");
        }

        Explode();
    }

    void Explode()
    {

        if (gridPlacement != null)
        {
            Vector3 snappedPos = new Vector3(
                Mathf.Round(transform.position.x / gridPlacement.gridSize) * gridPlacement.gridSize,
                Mathf.Round(transform.position.y / gridPlacement.gridSize) * gridPlacement.gridSize,
                Mathf.Round(transform.position.z / gridPlacement.gridSize) * gridPlacement.gridSize
            );

            //removes it from the gridplacment list so that other blocks may be placed in its location after it explodes 
            gridPlacement.occupiedPositions.Remove(snappedPos); 
        }

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Destroy explosion prefab after particles
            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(explosion, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(explosion, 2f); 
            }
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag(destructibleTag))
            {
                Destroy(hit.gameObject);
            }
        }
        SoundManager.Instance.PlaySound2D("Bomb");
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
