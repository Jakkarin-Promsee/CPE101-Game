using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Update HUD on stats change event
    public GameObject deadCanvas;
    public event Action OnStatsChanged;

    public float hp;
    public float shield;
    public float mana;

    public float timeToStartRegeneration = 10f;
    public float shieldRegenRate = 10f;
    private bool isRegeneratingShield = true;
    private float timeSinceOutOfCombat = 0f; // Timer to track time out of combat

    private SpriteRenderer spriteRenderer;
    public Color flashColor = Color.red;  // Color to flash
    public float flashDuration = 0.1f;      // Duration of flash in seconds
    private Color originalColor;            // Original color to reset back to

    public float MAX_HP;
    public float MAX_SHIELD;
    public float MAX_MANA;

    public int ending = -1;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;  // Store the original color
        // Initialize player stats
        MAX_HP = hp;
        MAX_SHIELD = shield;
        // MAX_MANA = mana;
        MAX_MANA = GetComponent<PlayerMovement>().dashCd;
        mana = MAX_MANA = GetComponent<PlayerMovement>().dashCd;
    }

    bool isResetMana = false;

    void Update()
    {
        // Regenerate health if player is out of combat for 10 secs
        if (timeSinceOutOfCombat < timeToStartRegeneration)
        {
            timeSinceOutOfCombat += Time.deltaTime;
        }
        else if (timeSinceOutOfCombat >= timeToStartRegeneration && !isRegeneratingShield)
        {
            isRegeneratingShield = true;
            StartCoroutine(RegenerateShield());
        }

        if (!GetComponent<PlayerMovement>().nextDash)
        {
            if (!isResetMana)
            {
                mana = 0;
                isResetMana = true;
            }

            if (mana < MAX_MANA)
                mana += Time.deltaTime;
            else
                mana = MAX_MANA;
            OnStatsChanged?.Invoke();
        }
        else
            isResetMana = false;
    }

    public void TakeDamage(float damage)
    {

        if (shield > 0)
        {
            shield -= damage;
            if (shield < 0) shield = 0;

            StartCoroutine(FlashRed());
        }
        else
        {
            hp -= damage;

            if (hp <= 0)
            {
                deadCanvas.SetActive(true);
            }


            StartCoroutine(FlashRed());
        }

        // In combat, stop shield regeneration
        timeSinceOutOfCombat = 0f;
        isRegeneratingShield = false;
        OnStatsChanged?.Invoke();
    }

    private IEnumerator RegenerateShield()
    {
        while (isRegeneratingShield && shield < MAX_SHIELD)
        {
            shield += shieldRegenRate;
            // Keep shield at MAX
            if (shield > MAX_SHIELD) shield = MAX_SHIELD;
            OnStatsChanged?.Invoke();
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
