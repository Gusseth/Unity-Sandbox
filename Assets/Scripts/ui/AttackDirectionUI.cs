using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackDirectionalUI
{
    public void UpdateTarget(BasicHitDirection direction);
}

public class AttackDirectionUI : MonoBehaviour, IAttackDirectionalUI
{
    [SerializeField] GameObject upIndicator;
    [SerializeField] GameObject downIndicator;
    [SerializeField] GameObject leftIndicator;
    [SerializeField] GameObject rightIndicator;
    GameObject[] indicators;

    public void UpdateTarget(BasicHitDirection direction)
    {
        ActivateIndicator(direction);
    }

    // Start is called before the first frame update
    void Start()
    {
        indicators = new GameObject[] { null, upIndicator, downIndicator, rightIndicator, leftIndicator };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ActivateIndicator(BasicHitDirection direction)
    {
        for (int i = 1; i < indicators.Length; i++)
        {
            if (i == (int)direction && direction != BasicHitDirection.None)
            {
                indicators[i].SetActive(true);
            } else
            {
                indicators[i].SetActive(false);
            }
        }
    }
}
