using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;

public static class TimeHelpers
{
    /// <summary>
    /// Invokes the function after a set amount of time
    /// </summary>
    /// <param name="monoBehaviour">The MonoBehaviour this coroutine will be attached to</param>
    /// <param name="function">The function to be invoked</param>
    /// <param name="seconds">Number of seconds to wait</param>
    /// <returns></returns>
    public static Coroutine InvokeCoroutine(this MonoBehaviour monoBehaviour, Action function, float seconds)
    {
        return monoBehaviour.StartCoroutine(InvokeDelayHelper(function, new WaitForSeconds(seconds)));
    }

    /// <summary>
    /// Invokes the function after a set amount of time with an additional WaitForSeconds
    /// </summary>
    /// <param name="monoBehaviour">The MonoBehaviour this coroutine will be attached to</param>
    /// <param name="function">The function to be invoked</param>
    /// <param name="delay">WaitForSeconds object</param>
    /// <returns></returns>
    public static Coroutine InvokeCoroutine(this MonoBehaviour monoBehaviour, Action function, WaitForSeconds delay)
    {
        return monoBehaviour.StartCoroutine(InvokeDelayHelper(function, delay));
    }

    private static IEnumerator InvokeDelayHelper(Action function, WaitForSeconds delay)
    {
        yield return delay;
        function();
    }

    /// <summary>
    /// Enumerates through a list with an option to add delay before and after an action is performed
    /// </summary>
    /// <typeparam name="T">The type used by the collection/enumerator</typeparam>
    /// <param name="monoBehaviour">The MonoBehaviour this coroutine will be attached to</param>
    /// <param name="function">The function that is called on the element</param>
    /// <param name="collection">The collection to enumerate through</param>
    /// <param name="initialDelay">The number of seconds waited before calling 'function'</param>
    /// <param name="endDelay">The number of seconds waited after calling 'function' before enumerating to the next element</param>
    /// <returns></returns>
    public static Coroutine StaggeredEnumerationCoroutine<T>(this MonoBehaviour monoBehaviour, Action<T> function, ICollection<T> collection,
        float initialDelay = 0, float endDelay = 0)
    {
        return monoBehaviour.StartCoroutine(StaggeredEnumerationHelper(function, collection, initialDelay, endDelay));
    }

    private static IEnumerator StaggeredEnumerationHelper<T>(Action<T> function, ICollection<T> collection, 
        float initialDelay, float endDelay)
    {
        foreach (T x in collection)
        {
            if (!initialDelay.Equals(0))
                yield return new WaitForSeconds(initialDelay);
            function(x);
            if (!endDelay.Equals(0))
                yield return new WaitForSeconds(endDelay);
        }
    }

    /// <summary>
    /// Enumerates through a list with an option to add delay before and after an action is performed
    /// </summary>
    /// <typeparam name="T">The type used by the collection/enumerator</typeparam>
    /// <param name="monoBehaviour">The MonoBehaviour this coroutine will be attached to</param>
    /// <param name="delayFunction">A function that extracts the initial and end delay times from each element of the collection</param>
    /// <param name="body">The main function that is called on the element</param>
    /// <param name="collection">The collection to enumerate through</param>
    /// <param name="callback">Function called after execution is done</param>
    /// <param name="sequentially">True if the enumeration ensures sequential operation, False if parallel operations are fine</param>
    /// <returns></returns>
    public static Coroutine StaggeredEnumerationCoroutine<T>(this MonoBehaviour monoBehaviour, Func<T, (float, float)> delayFunction,
        Action<T> body, ICollection<T> collection, Action callback = null, bool sequentially = true)
    {
        return monoBehaviour.StartCoroutine(
            StaggeredEnumerationHelper(body, delayFunction, collection, callback, monoBehaviour, sequentially)
            );
    }

    private static IEnumerator StaggeredEnumerationHelper<T>(Action<T> function, Func<T, (float, float)> delayFunction, 
        ICollection<T> collection, Action callback, MonoBehaviour mono, bool sequentially)
    {
        if (sequentially)
        {
            foreach (T x in collection)
            {
                // We wait for each routine to finish before proceeding
                yield return StaggeredCoroutineHelper(function, delayFunction, x);
            }
        } 
        else
        {
            foreach (T x in collection)
            {
                StaggeredCoroutineHelper(function, delayFunction, x);
            }
        }
        if (callback != null)
            callback();
    }

    public static Coroutine StaggeredCoroutine<T>(this MonoBehaviour monoBehaviour, Action<T> function, Func<T, 
        (float, float)> delayFunction, T x, Action callback = null)
    {
        return monoBehaviour.StartCoroutine(
            StaggeredCoroutineHelper(function, delayFunction, x, callback)
            );
    }

    public static Coroutine StaggeredCoroutine(this MonoBehaviour monoBehaviour, Action function,
        float initialDelay = 0, float endDelay = 0, Action callback = null)
    {
        return monoBehaviour.StartCoroutine(
            StaggeredCoroutineHelper(function, initialDelay, endDelay, callback)
            );
    }

    private static IEnumerator StaggeredCoroutineHelper<T>(Action<T> function, Func<T, (float, float)> delayFunction,
        T x, Action callback = null)
    {
        (float initialDelay, float endDelay) = delayFunction(x);

        if (initialDelay > 0)
            yield return new WaitForSeconds(initialDelay);
        function(x);
        if (endDelay > 0)
            yield return new WaitForSeconds(endDelay);
        if (callback != null)
            callback();
    }

    private static IEnumerator StaggeredCoroutineHelper(Action function, float initialDelay, float endDelay, Action callback)
    {
        if (initialDelay > 0)
            yield return new WaitForSeconds(initialDelay);
        function();
        if (endDelay > 0)
            yield return new WaitForSeconds(endDelay);
        if (callback != null)
            callback();
    }

    public static async Task WaitAsync(float time, bool useScaledTime = true)
    {
        float elapsed = 0;
        while (elapsed < time)
        {
            elapsed += useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            await Task.Yield();
        }
    }
}
