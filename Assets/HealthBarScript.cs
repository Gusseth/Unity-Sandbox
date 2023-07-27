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
    float foreFactor = 1;
    float backFactor = 1;

    public void UpdateTarget(int health, int maxHealth)
    {
        targetHealth = health / (float)maxHealth;
        frontElapsed = backElapsed = 0;
    }

    private void Start()
    {
        foreFactor = 1 / foregroundTime;
        backFactor = 1 / backgroundTime;
    }

    void Update()
    {
        if (frontElapsed < 1)
        {
            frontElapsed += Time.deltaTime * foreFactor;
            float foregroundFill = foregroundIndicator.fillAmount;
            foregroundIndicator.fillAmount = math.lerp(foregroundFill, targetHealth, frontElapsed);
            
        }
        if (backElapsed < 1)
        {
            backElapsed += Time.deltaTime * backFactor;
            float backgroundFill = backgroundIndicator.fillAmount;
            backgroundIndicator.fillAmount = math.lerp(backgroundFill, targetHealth, backElapsed);
        }
    }
}
