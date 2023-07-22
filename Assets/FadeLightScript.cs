using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FadeLightScript : MonoBehaviour
{
    [SerializeField] float timeInSeconds;
    new Light light;
    float t = 0;
    float start;
    float factor;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        start = light.range;
        factor = 1 / timeInSeconds; // dividing costs performance
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime * factor;
        light.range = math.lerp(start, 0, t);
    }
}
