using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ! Test
    public float hp = 100f;
    public float shield = 10f;
    private float timeToStartRegeneration = 10f;
    private bool isRegeneratingShield = true;
    private float timeSinceOutOfCombat = 0f; // Timer to track time out of combat
    // ! Test

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
        // Player dead
        if (hp <= 0) Destroy(gameObject);

        // Regenerate health if player is out of combat for 10 secs
        if(timeSinceOutOfCombat < timeToStartRegeneration){
            timeSinceOutOfCombat += Time.deltaTime;
            // print(timeSinceOutOfCombat);
        }else if(timeSinceOutOfCombat >= timeToStartRegeneration && !isRegeneratingShield){
            isRegeneratingShield = true;
            StartCoroutine(RegenerateShield());
        }
    }

    public void TakeDamage(float damage)
    {
        if(shield > 0){
            shield--;
        }else{
            hp--;
        }
        StartCoroutine(FlashRed());
        // In combat, stop shield regeneration
        timeSinceOutOfCombat = 0f;
        isRegeneratingShield = false;
    }

    private IEnumerator RegenerateShield(){
        while(isRegeneratingShield && shield < 10f){
            shield++;
            yield return new WaitForSeconds(1f);
        }
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
