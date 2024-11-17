using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Controller : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField] private Image healthImage;
    [SerializeField] private Image shieldImage;
    [SerializeField] private Image manaImage;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        // Null checking
        if (healthImage == null || shieldImage == null || manaImage == null)
            Debug.LogError("HUD UI elements not assigned.");

        // Attach event to playerController
        playerController.OnStatsChanged += UpdateHUD;
    }

    private void UpdateHUD(){
        healthImage.fillAmount = Mathf.Clamp01(playerController.hp / playerController.MAX_HP);
        shieldImage.fillAmount = Mathf.Clamp01(playerController.shield / playerController.MAX_SHIELD);
        manaImage.fillAmount = Mathf.Clamp01(playerController.mana / playerController.MAX_MANA);
    }
}
