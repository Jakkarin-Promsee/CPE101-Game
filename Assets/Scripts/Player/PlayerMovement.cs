using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the player
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
}
