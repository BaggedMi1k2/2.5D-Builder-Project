using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    public float spinSpeed = 200f;
    public float knockbackTorque = 50f; //this dosnt make sence anymore as my player canot spin 
    public float knockbackForce = 5f; // directional push

    void Update()
    {
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Calculate direction from saw to player
                Vector3 direction = (collision.transform.position - transform.position).normalized;

                // Apply torque based on collision direction
                // Using cross product to get perpendicular vector for torque
                Vector3 torqueDirection = Vector3.Cross(direction, Vector3.forward);

                // Apply the torque
                playerRb.AddTorque(torqueDirection * knockbackTorque, ForceMode.Impulse);

                playerRb.AddForce(direction * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}
