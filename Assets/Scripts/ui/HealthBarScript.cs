using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] Image foregroundIndicator;
    [SerializeField] Image backgroundIndicator;
    [SerializeField] float foregroundTime = 0.25f;
    [SerializeField] float backgroundTime = 2;
    float targetHealth = 0;
    float frontElapsed = 0;
    float backElapsed = 0;

    public void UpdateTarget(int health, int maxHealth)
    {
        targetHealth = health / (float)maxHealth;
        frontElapsed = backElapsed = 0;
    }

    private void Start()
    {

    }

    void Update()
    {
        if (frontElapsed < 1)
        {
            frontElapsed += Time.deltaTime * 1 / foregroundTime;
            float foregroundFill = foregroundIndicator.fillAmount;
            foregroundIndicator.fillAmount = math.lerp(foregroundFill, targetHealth, frontElapsed);
            
        }
        if (backElapsed < 1)
        {
            backElapsed += Time.deltaTime * 1 / backgroundTime;
            float backgroundFill = backgroundIndicator.fillAmount;
            backgroundIndicator.fillAmount = math.lerp(backgroundFill, targetHealth, backElapsed);
        }
    }
}
