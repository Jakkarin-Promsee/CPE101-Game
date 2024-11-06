using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the player
    public float dashRate = 4f;
    public float dashDistance = 5f; // Set the desired dash distance
    public float dashSpeed = 20f;   // Speed of the dash
    private bool isDashing = false;
    private Vector3 dashTarget;
    public float nextDashTime = 0f; // To handle fire rate
    public Camera mainCamera;    // Reference to the main camera

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        // Get the Rigidbody2D component for physics-based movement
        rb = GetComponent<Rigidbody2D>();

        // Check if the mainCamera is assigned; if not, find the main camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // Get input for WASD or arrow keys
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextDashTime) Dash();
        // Handle the dash movement
        if (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);

            // Stop dashing once we reach the target
            if (Vector3.Distance(transform.position, dashTarget) < 0.1f)
            {
                isDashing = false;
            }

        }
    }

    void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        // Update camera position to follow the player
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(rb.position.x, rb.position.y, mainCamera.transform.position.z);
        }
    }

    void Dash()
    {
        nextDashTime = Time.time + dashRate;

        // Get mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Ensure Z axis is consistent

        // Calculate direction to the mouse and normalize it
        Vector3 direction = (mousePosition - transform.position).normalized;

        // Set the dash target position
        dashTarget = transform.position + direction * dashDistance;
        isDashing = true;
    }
}
