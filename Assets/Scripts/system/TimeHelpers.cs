using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public static class TimeHelpers
{
    public static Coroutine InvokeCoroutine(this MonoBehaviour monoBehaviour, Action function, float seconds)
    {
        return monoBehaviour.StartCoroutine(InvokeDelayHelper(function, new WaitForSeconds(seconds)));
    }

    public static Coroutine InvokeCoroutine(this MonoBehaviour monoBehaviour, Action function, WaitForSeconds delay)
    {
        return monoBehaviour.StartCoroutine(InvokeDelayHelper(function, delay));
    }

    private static IEnumerator InvokeDelayHelper(Action function, WaitForSeconds delay)
    {
        yield return delay;
        function();
    }
}
