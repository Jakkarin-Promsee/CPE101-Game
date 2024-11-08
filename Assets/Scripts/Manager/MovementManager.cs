using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera mainCamera;    // Reference to the main camera

    // Movemet speed of the player, Max (moveSpeed,moveSpeed) * 1second
    public float moveSpeed = 5f;

    public float dashCd = 4f; // Dash Colldown
    public float dashDistance = 5f; // Dash Distant 
    public float dashSpeed = 20f; // Dash Speed


    public bool isforce = false;
    private bool isDashing = false;
    private Vector3 dashTarget;
    public float nextDashTime = 0f;
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
            Camera.main.aspect = 1920f / 1080f;
        }
    }

    void Update()
    {
        // Get input for WASD or arrow keys
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Check 'space' and 'right click'
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(1)) && Time.time > nextDashTime)
        {
            nextDashTime = Time.time + dashCd;
            Dash();
        }

        // Add dash movement to mouse position with dash distance
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
        // Move the player on keyboard position
        // if (!isforce) rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        if (!isforce) rb.velocity = movement * moveSpeed; // Move the player with velocity
        else isforce = false;

        // Update camera position to follow the player
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(rb.position.x, rb.position.y, mainCamera.transform.position.z);
        }
    }

    void Dash()
    {
        // Get mouse position in the screen
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Ensure Z axis is consistent

        // Calculate direction to the mouse and normalize it
        Vector3 direction = (mousePosition - transform.position).normalized;

        // Set the dash target position
        dashTarget = transform.position + direction * dashDistance;
        isDashing = true;
    }
}
