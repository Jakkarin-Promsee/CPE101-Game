using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float hp = 100;

    private SpriteRenderer spriteRenderer;
    public Color flashColor = Color.red;  // Color to flash
    public float flashDuration = 0.1f;      // Duration of flash in seconds
    private Color originalColor;            // Original color to reset back to


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;  // Store the original color
    }

    void Update()
    {
        if (hp <= 0) Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        // Set the sprite color to the flash color (white)
        spriteRenderer.color = flashColor;

        // Wait for the specified flash duration
        yield return new WaitForSeconds(flashDuration);

        // Reset the color back to the original color
        spriteRenderer.color = originalColor;
    }
}
