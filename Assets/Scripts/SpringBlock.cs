using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBlock : MonoBehaviour
{
    public float springForce = 20f;
    public float cooldownTime = 0.5f;
    public float compressAmount = 0.2f;
    public float returnSpeed = 5f;

    private Vector3 originalScale;
    private float lastBounceTime;
    private bool isCompressed = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Return to normal scale after compression
        if (isCompressed && Time.time > lastBounceTime + cooldownTime / 2)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                originalScale,
                Time.deltaTime * returnSpeed
            );

            if (Vector3.Distance(transform.localScale, originalScale) < 0.01f)
            {
                transform.localScale = originalScale;
                isCompressed = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            Time.time > lastBounceTime + cooldownTime)
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Apply spring force
                playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, 0f);
                playerRb.AddForce(Vector3.up * springForce, ForceMode.Impulse);

                // Visual compression effect
                transform.localScale = new Vector3(
                    originalScale.x,
                    originalScale.y - compressAmount,
                    originalScale.z
                );

                lastBounceTime = Time.time;
                isCompressed = true;
            }
        }
    }
}
