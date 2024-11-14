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
    private float nextDashTime = 0f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private float nextMovement = 0f;

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
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(1)) && Time.time > nextDashTime)
        {
            nextDashTime = Time.time + dashCd;
            Dash();
        }

        if (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, dashTarget) < 0.1f)
                isDashing = false;
        }
    }

    public void TakeKnockback(Vector3 force, float time)
    {
        nextMovement = Time.time + time;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        if (Time.time > nextMovement)
            rb.velocity = movement * moveSpeed;

        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(rb.position.x, rb.position.y, mainCamera.transform.position.z);
        }
    }

    void Dash()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        Vector3 direction = (mousePosition - transform.position).normalized;
        dashTarget = transform.position + direction * dashDistance;
        isDashing = true;
    }
}
