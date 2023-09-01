using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySword : MonoBehaviour
{
    IHitter hitter;
    bool waiting;

    private void Awake()
    {
        hitter ??= GetComponent<IHitter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hitter.Attacking)
        {
            hitter.PreAttack(Vector3.zero, null);
            waiting = true;
            TimeHelpers.InvokeCoroutine(this, () => hitter.PostAttack(), 1);
        }
    }
}
