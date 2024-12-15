using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
    public Slider slider;
    private SpiritKing bossScript;
    public GameObject hpBar;

    public void ShowHpBar()
    {
        hpBar.SetActive(true);
    }

    public void CloseHpBar()
    {
        hpBar.SetActive(false);
    }

    private void Start()
    {

        bossScript = GetComponent<SpiritKing>();

        bossScript.OnHpChanged += UpdateHpBar;
    }

    private void UpdateHpBar(float newValue)
    {
        slider.value = newValue;
    }
}
