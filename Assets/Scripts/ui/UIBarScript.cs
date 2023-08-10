using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

enum BarMode
{
    Health, Ke, Stamina
}

public class UIBarScript : MonoBehaviour
{
    [SerializeField] Image foregroundIndicator;
    [SerializeField] Image backgroundIndicator;
    [SerializeField] float foregroundTime = 0.25f;
    [SerializeField] float backgroundTime = 2;
    float target = 0;
    float frontElapsed = 0;
    float backElapsed = 0;

    public void UpdateTarget(int currentValue, int maxValue, bool showDecrease = true)
    {
        target = currentValue / (float)maxValue;
        frontElapsed = backElapsed = 0;

        if (!showDecrease)
        {
            backElapsed = 1;
            backgroundIndicator.fillAmount = target;
        }
    }

    private void Start()
    {
        
    }

    void Update()
    {
        if (frontElapsed < 1)
        {
            frontElapsed += Time.deltaTime / foregroundTime;
            float foregroundFill = foregroundIndicator.fillAmount;
            foregroundIndicator.fillAmount = math.lerp(foregroundFill, target, frontElapsed);
            
        }
        if (backElapsed < 1)
        {
            backElapsed += Time.deltaTime / backgroundTime;
            float backgroundFill = backgroundIndicator.fillAmount;
            backgroundIndicator.fillAmount = math.lerp(backgroundFill, target, backElapsed);
        }
    }
}
