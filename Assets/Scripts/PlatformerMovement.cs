using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerMovement : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float groundAcceleration = 20f;
    public float airAcceleration = 15f;
    public float rotationRecoverySpeed = 10f;

    //doublejump
    public float jumpForce = 12f;
    public float doubleJumpForce = 10f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    private bool canDoubleJump = false;
    private float lastGroundedTime;
    private float lastJumpPressedTime;

    //checkground
    public Transform groundCheck;
    public float groundRadius = 0.4f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody rb;
    private float horizontalInput;
    private bool jumpInput;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        lastGroundedTime = -coyoteTime - 1f;
        lastJumpPressedTime = -jumpBufferTime - 1f;
        rb.constraints = RigidbodyConstraints.FreezePositionZ |
                        RigidbodyConstraints.FreezeRotationX |
                        RigidbodyConstraints.FreezeRotationY;
    }

    void Update()
    {
        GetInput();
        CheckGround();
        HandleJumpInput();
        Flip();
    }

    void FixedUpdate()
    {
        HandleMovement();
        RecoverRotation();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButtonDown("Jump");

        if (jumpInput)
        {
            lastJumpPressedTime = Time.time;
        }
    }
    void RecoverRotation()
    {
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    void CheckGround()
    {
        // Visualize the ground check in Scene view
        Debug.DrawRay(groundCheck.position, Vector3.down * groundRadius, Color.red);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            canDoubleJump = true;
        }
    }

    void Jump(float force)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, 0); // Reset vertical velocity
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    void HandleJumpInput()
    {
        // Ground jump (with coyote time)
        if ((isGrounded || Time.time - lastGroundedTime <= coyoteTime) && jumpInput)
        {
            Jump(jumpForce);
            canDoubleJump = true;
        }
        // Double jump (if available)
        else if (!isGrounded && canDoubleJump && jumpInput)
        {
            Jump(doubleJumpForce);
            canDoubleJump = false;
        }
    }

    void HandleMovement()
    {
        float targetSpeed = horizontalInput * moveSpeed;
        float acceleration = isGrounded ? groundAcceleration : airAcceleration;

        RaycastHit hit;
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundRadius * 1.1f, groundLayer))
        {
            PhysicMaterial mat = hit.collider.sharedMaterial;
            if (mat != null)
            {
                if (mat.name.Contains("Icy")) // If on icy surface
                {
                    // Reduce acceleration for more slippery feel
                    acceleration = isGrounded ? groundAcceleration * 0.5f : airAcceleration;
                }
            }
        }

        // Calculate force to reach target speed
        float speedDifference = targetSpeed - rb.velocity.x;
        float movementForce = speedDifference * acceleration;

        rb.AddForce(Vector3.right * movementForce);
    }

    void Flip()
    {
        if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
        {
            facingRight = !facingRight;
            transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        }
    }

    // Visualize ground check and wall check in editor
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        Gizmos.color = Color.blue;
    }

}
