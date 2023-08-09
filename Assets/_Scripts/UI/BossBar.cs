using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxBossHealth(float bossHealth)
    {
        slider.maxValue = bossHealth;
        slider.value = bossHealth;
    }

    public void SetBossHealth(float bossHealth)
    {
        slider.value = bossHealth;
    }
}
