using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Controller : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerController;

    // Status
    [Header("Images")]
    [SerializeField] private Image healthIcon;
    [SerializeField] private Image shieldIcon;
    [SerializeField] private Image manaIcon;
    [SerializeField] private Image healthImage;
    [SerializeField] private Image shieldImage;
    [SerializeField] private Image manaImage;

    // Show actual num
    [SerializeField] private Text healthText;
    [SerializeField] private Text shieldText;
    [SerializeField] private Text manaText;

    // Sprite to change image based on status;
    [Header("Health sprites")]
    [SerializeField] private Sprite fullHealthSprite;
    [SerializeField] private Sprite halfHealthSprite;
    [SerializeField] private Sprite lowHealthSprite;
    [Header("Shield sprites")]
    [SerializeField] private Sprite fullShieldSprite;
    [SerializeField] private Sprite halfShieldSprite;
    [SerializeField] private Sprite lowShieldSprite;
    [Header("Mana sprites")]
    [SerializeField] private Sprite fullManaSprite;
    [SerializeField] private Sprite halfManaSprite;
    [SerializeField] private Sprite lowManaSprite;

    void Start()
    {
        // Find player
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();

        // Null checking
        if (healthImage == null || shieldImage == null || manaImage == null)
            Debug.LogError("HUD UI elements not assigned.");

        // Attach event to playerController
        playerController.OnStatsChanged += UpdateHUD;

        // Initialize HUD
        UpdateHUD();
    }

    private void UpdateHUD(){
        changeIcon();
        changeText();
        changeBarLength();
    }

    private void changeIcon(){
        // Health icon
        if (playerController.hp/playerController.MAX_HP >= 0.7f)
            healthIcon.sprite = fullHealthSprite;
        else if (playerController.hp/playerController.MAX_HP >= 0.3f)
            healthIcon.sprite = halfHealthSprite;
        else
            healthIcon.sprite = lowHealthSprite;
        

        // Shield icon
        if (playerController.shield/playerController.MAX_SHIELD >= 0.7f)
            shieldIcon.sprite = fullShieldSprite;
        else if (playerController.shield/playerController.MAX_SHIELD >= 0.3f)
            shieldIcon.sprite = halfShieldSprite;
        else
            shieldIcon.sprite = lowShieldSprite;

        // Mana icon
        if (playerController.mana/playerController.MAX_MANA >= 0.7f)
            manaIcon.sprite = fullManaSprite;
        else if (playerController.mana/playerController.MAX_MANA >= 0.3f)
            manaIcon.sprite = halfManaSprite;
        else
            manaIcon.sprite = lowManaSprite;
    }

    private void changeText(){
        healthText.text = playerController.hp + " / " + playerController.MAX_HP;
        shieldText.text = playerController.shield + " / " + playerController.MAX_SHIELD;
        manaText.text = playerController.mana + " / " + playerController.MAX_MANA;
    }

    private void changeBarLength(){
        // Bar length
        healthImage.fillAmount = Mathf.Clamp01(playerController.hp / playerController.MAX_HP);
        shieldImage.fillAmount = Mathf.Clamp01(playerController.shield / playerController.MAX_SHIELD);
        manaImage.fillAmount = Mathf.Clamp01(playerController.mana / playerController.MAX_MANA);
    }
}
